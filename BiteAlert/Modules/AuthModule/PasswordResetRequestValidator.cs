// Ignore Spelling: Validator

using FluentValidation;

namespace BiteAlert.Modules.AuthModule;

public class PasswordResetRequestValidator : AbstractValidator<PasswordResetRequest>
{
    public PasswordResetRequestValidator()
    {
        RuleFor(request => request.UserId)
            .NotEmpty().WithMessage("UserId cannot be empty.");

        RuleFor(request => request.PasswordResetToken)
            .NotEmpty().WithMessage("Password reset token cannot be empty.");

        RuleFor(request => request.NewPassword)
            .NotEmpty().WithMessage("New password cannot be empty.")
            .MinimumLength(8)
                .WithMessage("New password must be at least 8 characters long.")
            .Matches(@"[A-Z]")
                .WithMessage("New password must have at least one uppercase letter ('A'-'Z').")
            .Matches(@"[a-z]")
                .WithMessage("New password must have at least one lowercase letter ('a'-'z').")
            .Matches(@"\d")
                .WithMessage("New password must have at least one digit ('0'-'9').")
            .Matches(@"\W")
                .WithMessage("New password must have at least one non-alphanumeric character.");

        RuleFor(request => request.ConfirmPassword)
            .NotEmpty().WithMessage("Confirm password cannot be empty.")
            .Equal(request => request.NewPassword).WithMessage("Passwords do not match.");
    }
}
