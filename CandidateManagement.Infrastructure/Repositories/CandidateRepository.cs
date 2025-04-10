using CandidateManagement.Application.Repositories;
using CandidateManagement.Domain.Entities;
using CandidateManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Infrastructure.Repositories;

public class CandidateRepository(AppDbContext context) : ICandidateRepository
{
    public async Task<Candidate?> GetByEmailAsync(string email)
    {
        return await context.Candidates.FirstOrDefaultAsync(candidate => candidate.Email == email.ToLower());
    }

    public async Task AddAsync(Candidate candidate)
    {
        await context.Candidates.AddAsync(candidate);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Candidate candidate)
    {
        context.Candidates.Update(candidate);
        await context.SaveChangesAsync();
    }
}
