namespace CandidateManagement.Application.DTOs;

public class CandidateDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public TimeOnly? StartCallTime { get; set; }
    public TimeOnly? EndCallTime { get; set; }
    public string? LinkedInProfileUrl { get; set; }
    public string? GitHubProfileUrl { get; set; }
    public string Comment { get; set; } = string.Empty;
}
