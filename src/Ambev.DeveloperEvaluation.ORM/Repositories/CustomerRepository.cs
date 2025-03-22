using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DefaultContext _context;

        public CustomerRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync(cancellationToken);
            return customer;
        }
    }
}
