using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IBranchRepository
{
    Task<Branch> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken);
    Task<Branch> CreateAsync(Branch branch, CancellationToken cancellationToken);
}