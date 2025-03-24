using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.SaleNumber)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Sale number is required and must not exceed 50 characters");

        RuleFor(x => x.SaleDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Sale date must be in the past or present");

        RuleFor(x => x.BranchId)
            .NotEmpty()
            .WithMessage("Branch ID is required");

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Customer ID is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");
            
        RuleFor(x => x.Items)
            .Must(items => items.GroupBy(i => i.ProductId).All(g => g.Sum(i => i.Quantity) <= 20))
            .WithMessage("A sale cannot contain more than 20 identical items.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required");

            item.RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero");

            item.RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("Unit price must be greater than zero");

            item.RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(x => x.Quantity * x.UnitPrice)
                .WithMessage("Discount must be between 0 and the total item price");
        });
    }
}