using DotNetCore.EnterpriseTemplate.Domain.Entities;

namespace DotNetCore.EnterpriseTemplate.Domain.Repositories;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken);
}