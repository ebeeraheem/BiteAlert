// Ignore Spelling: Validator

using FluentValidation;

namespace BiteAlert.Modules.AuthModule;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email cannot be empty.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(8)
                .WithMessage("Passwords must be at least 8 characters.")
            .Matches(@"[A-Z]")
                .WithMessage("Passwords must have at least one uppercase letter ('A'-'Z').")
            .Matches(@"[a-z]")
                .WithMessage("Passwords must have at least one lowercase letter ('a'-'z').")
            .Matches(@"\d")
                .WithMessage("Passwords must have at least one digit ('0'-'9').")
            .Matches(@"\W")
                .WithMessage("Passwords must have at least one non-alphanumeric character.");
    }
}
