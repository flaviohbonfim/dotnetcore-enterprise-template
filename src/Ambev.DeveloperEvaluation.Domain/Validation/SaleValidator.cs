using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation
{
    public class SaleValidator : AbstractValidator<Sale>
    {
        public SaleValidator()
        {
            RuleFor(x => x.SaleNumber)
                .NotEmpty().WithMessage("Sale number is required.");

            RuleFor(x => x.SaleDate)
                .NotNull().WithMessage("Sale date is required.");

            RuleForEach(x => x.Items)
                .ChildRules(items =>
                {
                    items.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
                    items.RuleFor(i => i.UnitPrice)
                        .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
                });

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than zero.");
        }
    }
}
