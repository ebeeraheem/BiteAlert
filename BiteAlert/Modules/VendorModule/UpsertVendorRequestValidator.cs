// Ignore Spelling: Validator Upsert

using FluentValidation;

namespace BiteAlert.Modules.VendorModule;

public class UpsertVendorRequestValidator : AbstractValidator<UpsertVendorRequest>
{
    public UpsertVendorRequestValidator()
    {
        RuleFor(request => request.BusinessName)
            .NotEmpty().WithMessage("Business name is required.");

        RuleFor(request => request.BusinessTagline)
            .MaximumLength(100)
            .WithMessage("Business tagline cannot exceed 100 characters.")
            .When(request => !string.IsNullOrEmpty(request.BusinessTagline));

        RuleFor(request => request.BusinessDescription)
            .NotEmpty().WithMessage("Business description is required.")
            .MaximumLength(1000)
            .WithMessage("Business description cannot exceed 1000 characters.");

        RuleFor(request => request.BusinessAddress)
            .NotEmpty().WithMessage("Business address is required.")
            .MaximumLength(250)
            .WithMessage("Business address cannot exceed 250 characters.");

        RuleFor(request => request.BusinessEmail)
            .EmailAddress().WithMessage("Invalid email address format.")
            .When(request => !string.IsNullOrEmpty(request.BusinessEmail));

        RuleFor(request => request.BusinessPhoneNumber)
            .Matches(@"^\+?\d{10,15}$")
            .WithMessage("Business phone number must be between 10 and 15 digits and may start with a '+' sign.")
            .When(request => !string.IsNullOrEmpty(request.BusinessPhoneNumber));
    }
}
