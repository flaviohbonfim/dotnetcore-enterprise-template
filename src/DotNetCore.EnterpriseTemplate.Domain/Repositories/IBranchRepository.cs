using DotNetCore.EnterpriseTemplate.Domain.Entities;

namespace DotNetCore.EnterpriseTemplate.Domain.Repositories;

public interface IBranchRepository
{
    Task<Branch> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken);
    Task<Branch> CreateAsync(Branch branch, CancellationToken cancellationToken);
}