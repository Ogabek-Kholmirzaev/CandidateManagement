using System.Text.RegularExpressions;
using CandidateManagement.Application.DTOs;
using FluentValidation;

namespace CandidateManagement.Application.Validators;

public class CandidateDtoValidator : AbstractValidator<CandidateDto>
{
    public CandidateDtoValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("A valid email address is required");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
            .Matches(new Regex(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$"))
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber))
            .WithMessage("Please enter a valid phone number");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required");

        RuleFor(x => x.LinkedInProfileUrl)
            .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Please enter a valid LinkedIn URL");

        RuleFor(x => x.GitHubProfileUrl)
            .Must(uri => string.IsNullOrWhiteSpace(uri) || Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage("Please enter a valid GitHub URL");

        When(x => x.StartCallTime.HasValue, () =>
        {
            RuleFor(x => x.EndCallTime)
                .NotNull().WithMessage("End call time is required when start call time is provided");
        });

        When(x => x.EndCallTime.HasValue, () =>
        {
            RuleFor(x => x.StartCallTime)
                .NotNull().WithMessage("Start call time is required when end call time is provided");
        });

        When(x => x.StartCallTime.HasValue && x.EndCallTime.HasValue, () => {
            RuleFor(x => x.EndCallTime)
                .Must((candidate, endTime) => endTime > candidate.StartCallTime)
                .WithMessage("End call time must be later than start call time");
        });
    }
}
