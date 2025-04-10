using CandidateManagement.Domain.Entities;

namespace CandidateManagement.Application.Repositories;

public interface ICandidateRepository
{
    Task<Candidate?> GetByEmailAsync(string email);
    Task AddAsync(Candidate candidate);
    Task UpdateAsync(Candidate candidate);
}
