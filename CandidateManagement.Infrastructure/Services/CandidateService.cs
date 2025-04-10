using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Repositories;
using CandidateManagement.Application.Services;
using CandidateManagement.Domain.Entities;

namespace CandidateManagement.Infrastructure.Services;

public class CandidateService(ICandidateRepository repository) : ICandidateService
{
    public async Task<Candidate> AddOrUpdateAsync(CandidateDto dto)
    {
        var candidate = await repository.GetByEmailAsync(dto.Email);
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
        }

        return candidate;
    }
}
