// Ignore Spelling: Validator

using FluentValidation;

namespace BiteAlert.Modules.Authentication;

public class UserProfileRequestValidator : AbstractValidator<UserProfileRequest>
{
    public UserProfileRequestValidator()
    {
        RuleFor(request => request.FirstName)
            .Matches(@"^[a-zA-Z]+$")
            .WithMessage("First name can only contain alphabetic characters.")
            .When(request => !string.IsNullOrEmpty(request.FirstName));

        RuleFor(request => request.LastName)
            .Matches(@"^[a-zA-Z]+$")
            .WithMessage("Last name can only contain alphabetic characters.")
            .When(request => !string.IsNullOrEmpty(request.LastName));

        RuleFor(request => request.UserName)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain alphanumeric characters and underscores.")
            .Length(3, 20).WithMessage("Username must be between 3 and 20 characters long.")
            .When(request => !string.IsNullOrEmpty(request.UserName));

        RuleFor(request => request.PhoneNumber)
            .Matches(@"^\+?\d{10,15}$")
            .WithMessage("Phone number must be between 10 and 15 digits and may start with a '+' sign.")
            .When(request => !string.IsNullOrEmpty(request.PhoneNumber));

        RuleFor(request => request.DateOfBirth)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date of birth cannot be in the future.")
            .When(request => request.DateOfBirth.HasValue);

        RuleFor(request => request.ProfilePictureUrl)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Profile picture URL must be a valid URL.")
            .When(request => !string.IsNullOrEmpty(request.ProfilePictureUrl));
    }
}
