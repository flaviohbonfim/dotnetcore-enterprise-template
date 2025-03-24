using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow);

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty();

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0);

            item.RuleFor(x => x.UnitPrice)
                .GreaterThan(0);

            item.RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(x => x.UnitPrice * x.Quantity);
        });
    }
}