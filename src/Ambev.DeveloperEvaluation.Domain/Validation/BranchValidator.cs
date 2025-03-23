using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation
{
    public class BranchValidator : AbstractValidator<Branch>
    {
        public BranchValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Branch name is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Branch address is required.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Branch phone number is required.");

            RuleFor(x => x.Manager)
                .NotEmpty().WithMessage("Branch manager name is required.");
        }
    }
}
