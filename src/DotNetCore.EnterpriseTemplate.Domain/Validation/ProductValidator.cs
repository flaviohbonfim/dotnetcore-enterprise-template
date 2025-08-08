using DotNetCore.EnterpriseTemplate.Domain.Entities;
using FluentValidation;

namespace DotNetCore.EnterpriseTemplate.Domain.Validation
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Product description is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Product price must be greater than zero.");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("Product SKU is required.");
        }
    }
}
