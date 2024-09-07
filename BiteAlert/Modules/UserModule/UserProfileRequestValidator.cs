// Ignore Spelling: Validator

using FluentValidation;
using System.Globalization;

namespace BiteAlert.Modules.UserModule;

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
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(BeAValidDate).WithMessage("Date of birth must be a valid date.")
            .Must(BeAtLeast13YearsOld).WithMessage("User must be at least 13 years old.")
            .When(request => !string.IsNullOrEmpty(request.DateOfBirth));
    }

    // Helper method to check if the date is valid
    private bool BeAValidDate(string? dateOfBirth)
    {
        return DateTime.TryParse(dateOfBirth, new CultureInfo("en-GB"), out _);
    }

    // Helper method to check if the user is at least 13 years old
    private bool BeAtLeast13YearsOld(string? dateOfBirth)
    {
        if (DateTime.TryParse(dateOfBirth, new CultureInfo("en-GB"), out DateTime parsedDate))
        {
            var minAgeDate = DateTime.UtcNow.AddYears(-13);
            return parsedDate <= minAgeDate;
        }
        return false;
    }
}
