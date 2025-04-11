using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Repositories;
using CandidateManagement.Application.Services;
using CandidateManagement.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CandidateManagement.Infrastructure.Services;

public class CandidateService(
    ICandidateRepository repository,
    IMemoryCache cache,
    ILogger<CandidateService> logger)
    : ICandidateService
{
    public async Task<Candidate> AddOrUpdateAsync(CandidateDto dto)
    {
        var cacheKey = $"candidate:{dto.Email.ToLower()}";
        if (!cache.TryGetValue(cacheKey, out Candidate? candidate))
        {
            logger.LogInformation("Cache not exists for candidate with email: {Email}", dto.Email);
            candidate = await repository.GetByEmailAsync(dto.Email);
        }

        if (candidate == null)
        {
            candidate = new Candidate(
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.PhoneNumber,
                dto.StartCallTime,
                dto.EndCallTime,
                dto.LinkedInProfileUrl,
                dto.GitHubProfileUrl,
                dto.Comment);

            await repository.AddAsync(candidate);
            logger.LogInformation("Candidate added with email: {Email}", candidate.Email);
        }
        else
        {
            candidate.Update(
                dto.FirstName,
                dto.LastName,
                dto.PhoneNumber,
                dto.StartCallTime,
                dto.EndCallTime,
                dto.LinkedInProfileUrl,
                dto.GitHubProfileUrl,
                dto.Comment);

            await repository.UpdateAsync(candidate);
            logger.LogInformation("Candidate updated with email: {Email}", candidate.Email);
        }

        cache.Set(cacheKey, candidate, TimeSpan.FromMinutes(10));

        return candidate;
    }
}
