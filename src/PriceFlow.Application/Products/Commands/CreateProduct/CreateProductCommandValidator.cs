using FluentValidation;

namespace PriceFlow.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(64)
            .Matches("^[A-Za-z0-9-]+$").WithMessage("SKU may contain only letters, digits and '-'.");
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Cost).GreaterThan(0);
        RuleFor(x => x.MinMarginPercent).InclusiveBetween(0, 500);
    }
}
