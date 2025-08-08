using DotNetCore.EnterpriseTemplate.Domain.Entities;
using FluentValidation;

namespace DotNetCore.EnterpriseTemplate.Domain.Validation
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Customer name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Customer email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Customer phone number is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Customer address is required.");
        }
    }
}
