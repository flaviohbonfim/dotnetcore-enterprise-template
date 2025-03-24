using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface ICustomerRepository
{
    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken);
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken);
}