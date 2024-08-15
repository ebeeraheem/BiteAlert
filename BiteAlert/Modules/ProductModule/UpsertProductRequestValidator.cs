// Ignore Spelling: Upsert Validator

using FluentValidation;

namespace BiteAlert.Modules.ProductModule;

public class UpsertProductRequestValidator : AbstractValidator<UpsertProductRequest>
{
    public UpsertProductRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Product name is required.");

        RuleFor(request => request.Description)
            .MaximumLength(1000)
            .WithMessage("Description cannot be longer than 1000 characters.");

        RuleFor(request => request.Price)
            .InclusiveBetween(1.00m, 10000000.00m)
            .WithMessage("Price must be between ₦1 and ₦10,000,000.00");

        RuleFor(request => request.ImageUrl)
            .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
            .WithMessage("Product image URL must be a valid URL.");
    }
}
