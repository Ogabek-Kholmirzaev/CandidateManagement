using CandidateManagement.Application.DTOs;
using CandidateManagement.Application.Repositories;
using CandidateManagement.Domain.Entities;
using CandidateManagement.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace CandidateManagement.Tests.Services;

public class CandidateServiceTests
{
    private readonly Mock<ICandidateRepository> _repositoryMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly Mock<ICacheEntry> _cacheEntryMock;
    private readonly Mock<ILogger<CandidateService>> _loggerMock;
    private readonly CandidateService _service;
    private readonly CandidateDto _dto;

    public CandidateServiceTests()
    {
        _repositoryMock = new Mock<ICandidateRepository>();
        _cacheMock = new Mock<IMemoryCache>();
        _cacheEntryMock = new Mock<ICacheEntry>();
        _loggerMock = new Mock<ILogger<CandidateService>>();

        _cacheMock
                .Setup(x => x.CreateEntry(It.IsAny<object>()))
                .Returns(_cacheEntryMock.Object);

        _service = new CandidateService(_repositoryMock.Object, _cacheMock.Object, _loggerMock.Object);

        _dto = new CandidateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john_doe@test.com",
            PhoneNumber = "+998999999999",
            StartCallTime = new TimeOnly(10, 0),
            EndCallTime = new TimeOnly(10, 30),
            LinkedInProfileUrl = "https://linkedin.com/in/john-doe",
            GitHubProfileUrl = "https://github.com/john-doe",
            Comment = "No Comments"
        };
    }

    [Fact]
    public async Task AddOrUpdateAsync_WhenCandidateNotInCacheButExistsInDb_UpdatesCandidate()
    {
        // Arrange
        var cacheKey = $"candidate:{this._dto.Email.ToLower()}";
        _cacheMock.Setup(m => m.TryGetValue(cacheKey, out It.Ref<object?>.IsAny)).Returns(false);

        var existingCandidate = new Candidate(
            "John",
            "Smith",
            _dto.Email,
            "+998777777777",
            new TimeOnly(9, 30),
            new TimeOnly(10, 0),
            "https://linkedin.com/in/john-smith",
            "https://github.com/john-smith",
            "Comment");

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(_dto.Email))
            .ReturnsAsync(existingCandidate);

        // Act
        var result = await _service.AddOrUpdateAsync(_dto);

        // Assert
        result.FirstName.Should().Be(_dto.FirstName);
        result.LastName.Should().Be(_dto.LastName);
        result.Email.Should().Be(_dto.Email.ToLower());
        result.PhoneNumber.Should().Be(_dto.PhoneNumber);
        result.StartCallTime.Should().Be(_dto.StartCallTime);
        result.EndCallTime.Should().Be(_dto.EndCallTime);
        result.LinkedInProfileUrl.Should().Be(_dto.LinkedInProfileUrl);
        result.GithubProfileUrl.Should().Be(_dto.GitHubProfileUrl);
        result.Comment.Should().Be(_dto.Comment);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidate>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Never);
        _cacheMock.Verify(c => c.CreateEntry(cacheKey), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WhenCandidateNotInCacheAndNotInDb_AddsNewCandidate()
    {
        // Arrange
        var cacheKey = $"candidate:{_dto.Email.ToLower()}";

        _cacheMock
            .Setup(m => m.TryGetValue(cacheKey, out It.Ref<object?>.IsAny))
            .Returns(false);

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(_dto.Email))
            .ReturnsAsync((Candidate?)null);

        Candidate? addedCandidate = null;

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Candidate>()))
            .Callback<Candidate>(c => addedCandidate = c)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.AddOrUpdateAsync(_dto);

        // Assert
        result.FirstName.Should().Be(_dto.FirstName);
        result.LastName.Should().Be(_dto.LastName);
        result.Email.Should().Be(_dto.Email.ToLower());
        result.PhoneNumber.Should().Be(_dto.PhoneNumber);
        result.StartCallTime.Should().Be(_dto.StartCallTime);
        result.EndCallTime.Should().Be(_dto.EndCallTime);
        result.LinkedInProfileUrl.Should().Be(_dto.LinkedInProfileUrl);
        result.GithubProfileUrl.Should().Be(_dto.GitHubProfileUrl);
        result.Comment.Should().Be(_dto.Comment);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidate>()), Times.Never);
        _cacheMock.Verify(c => c.CreateEntry(cacheKey), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_WhenCandidateInCache_UsesCache()
    {
        // Arrange
        var cacheKey = $"candidate:{_dto.Email.ToLower()}";
        var cachedCandidate = new Candidate(
            "John",
            "Smith",
            _dto.Email,
            "+998777777777",
            new TimeOnly(12, 30),
            new TimeOnly(13, 0),
            "https://linkedin.com/in/johnsmith",
            "https://github.com/johnsmith",
            "Old comment");

        object expectedValue = cachedCandidate;

        _cacheMock
            .Setup(m => m.TryGetValue(cacheKey, out expectedValue!))
            .Returns(true);

        // Act
        var result = await _service.AddOrUpdateAsync(_dto);

        // Assert
        result.FirstName.Should().Be(_dto.FirstName);
        result.LastName.Should().Be(_dto.LastName);
        result.Email.Should().Be(_dto.Email.ToLower());
        result.PhoneNumber.Should().Be(_dto.PhoneNumber);
        result.StartCallTime.Should().Be(_dto.StartCallTime);
        result.EndCallTime.Should().Be(_dto.EndCallTime);
        result.LinkedInProfileUrl.Should().Be(_dto.LinkedInProfileUrl);
        result.GithubProfileUrl.Should().Be(_dto.GitHubProfileUrl);
        result.Comment.Should().Be(_dto.Comment);

        _repositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Candidate>()), Times.Once);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Candidate>()), Times.Never);
        _cacheMock.Verify(c => c.CreateEntry(cacheKey), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_NormalizesEmail()
    {
        // Arrange
        var dtoWithMixedCaseEmail = new CandidateDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "John.Doe@Example.com",
            PhoneNumber = "+998917777777"
        };

        var cacheKey = $"candidate:{dtoWithMixedCaseEmail.Email.ToLower()}";

        _cacheMock
            .Setup(m => m.TryGetValue(cacheKey, out It.Ref<object?>.IsAny))
            .Returns(false);

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(dtoWithMixedCaseEmail.Email))
            .ReturnsAsync((Candidate?)null);

        // Act
        var result = await _service.AddOrUpdateAsync(dtoWithMixedCaseEmail);

        // Assert
        result.Email.Should().Be("john.doe@example.com");

        _cacheMock.Verify(c => c.CreateEntry("candidate:john.doe@example.com"), Times.Once);
    }

    [Fact]
    public async Task AddOrUpdateAsync_SetsCorrectCacheDuration()
    {
        // Arrange
        var cacheKey = $"candidate:{_dto.Email.ToLower()}";
        TimeSpan? capturedExpiration = null;

        _cacheMock
            .Setup(m => m.TryGetValue(cacheKey, out It.Ref<object?>.IsAny))
            .Returns(false);

        _repositoryMock
            .Setup(r => r.GetByEmailAsync(_dto.Email))
            .ReturnsAsync((Candidate?)null);

        _cacheEntryMock
            .SetupSet(e => e.AbsoluteExpirationRelativeToNow = It.IsAny<TimeSpan>())
            .Callback<TimeSpan?>(timeSpan => capturedExpiration = timeSpan);

        // Act
        await _service.AddOrUpdateAsync(_dto);

        // Assert
        capturedExpiration.Should().Be(TimeSpan.FromMinutes(10));

        _cacheMock.Verify(c => c.CreateEntry(cacheKey), Times.Once);
    }
}
