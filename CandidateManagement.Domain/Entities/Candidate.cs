namespace CandidateManagement.Domain.Entities;

public class Candidate
{
    public int Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public TimeOnly? StartCallTime { get; private set; }
    public TimeOnly? EndCallTime { get; private set; }
    public string? LinkedInProfileUrl { get; private set; }
    public string? GithubProfileUrl { get; private set; }
    public string Comment { get; private set; }

    private Candidate() { }

    public Candidate(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber,
        TimeOnly? startCallTime,
        TimeOnly? endCallTime,
        string? linkedInProfileUrl,
        string? githubProfileUrl,
        string comment)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        StartCallTime = startCallTime;
        EndCallTime = endCallTime;
        LinkedInProfileUrl = linkedInProfileUrl;
        GithubProfileUrl = githubProfileUrl;
        Comment = comment;
    }
}
