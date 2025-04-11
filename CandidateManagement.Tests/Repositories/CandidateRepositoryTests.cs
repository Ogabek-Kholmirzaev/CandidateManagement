using CandidateManagement.Application.Repositories;
using CandidateManagement.Domain.Entities;
using CandidateManagement.Infrastructure.Data;
using CandidateManagement.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace CandidateManagement.Tests.Repositories;

public class CandidateRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly ICandidateRepository _repository;

    public CandidateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CandidateRepository(_context);
    }

    [Fact]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsCandidateWithMatchingEmail()
    {
        // Arrange
        var email = "john_doe@test.com";
        var candidate = new Candidate(
            "John",
            "Doe",
            email,
            "+998999999999",
            new TimeOnly(10, 0),
            new TimeOnly(10, 30),
            "https://linkedin.com/in/john-doe",
            "https://github.com/john-doe",
            "No comments");

        await _context.Candidates.AddAsync(candidate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(candidate);
    }

    [Fact]
    public async Task GetByEmailAsync_NonExistingEmail_ReturnsNull()
    {
        // Arrange
        var email = "john_doe@test.com";

        // Act
        var result = await _repository.GetByEmailAsync(email);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_EmailCaseInsensitive_ReturnsMatchingCandidate()
    {
        // Arrange
        var lowerCaseEmail = "test@example.com";
        var upperCaseEmail = "TEST@EXAMPLE.COM";

        var candidate = new Candidate(
            "John",
            "Doe",
            lowerCaseEmail,
            "+998999999999",
            new TimeOnly(10, 0),
            new TimeOnly(10, 30),
            "https://linkedin.com/in/john-doe",
            "https://github.com/john-doe",
            "No comments");

        await _context.Candidates.AddAsync(candidate);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByEmailAsync(upperCaseEmail);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(candidate);
    }

    [Fact]
    public async Task AddAsync_ValidCandidate_AddToDb()
    {
        // Arrange
        var candidate = new Candidate(
            "John",
            "Doe",
            "john_doe@test.com",
            "+998999999999",
            new TimeOnly(10, 0),
            new TimeOnly(10, 30),
            "https://linkedin.com/in/john-doe",
            "https://github.com/john-doe",
            "No comments");

        await _repository.AddAsync(candidate);

        // Act
        var result = await _context.Candidates.FindAsync(candidate.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(candidate);
    }

    [Fact]
    public async Task AddAsync_InValidCandidate_ReturnsException()
    {
        // Arrange
        var candidate = new Candidate(
            "John",
            "Doe",
            "john_doe@test.com",
            "+998999999999",
            new TimeOnly(10, 0),
            new TimeOnly(10, 30),
            "https://linkedin.com/in/john-doe",
            "https://github.com/john-doe",
            "No comments");

        await _context.Candidates.AddAsync(candidate);
        await _context.SaveChangesAsync();

        // Act
        var act = async () => await _repository.AddAsync(candidate);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ExistingCandidate_UpdateInDb()
    {
        var candidate = new Candidate(
            "John",
            "Doe",
            "john_doe@test.com",
            "+998999999999",
            new TimeOnly(10, 0),
            new TimeOnly(10, 30),
            "https://linkedin.com/in/john-doe",
            "https://github.com/john-doe",
            "No comments");

        await _context.Candidates.AddAsync(candidate);
        await _context.SaveChangesAsync();

        // Act
        candidate.Update("Robert", "Albert", null, null, null, null, null, "");
        await _repository.UpdateAsync(candidate);

        // Assert
        var updatedCandidate = await _context.Candidates.FindAsync(candidate.Id);

        updatedCandidate.Should().NotBeNull();
        updatedCandidate.Should().BeEquivalentTo(candidate);
    }
}
