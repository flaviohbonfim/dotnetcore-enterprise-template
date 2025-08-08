using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore.EnterpriseTemplate.ORM.Repositories
{
    public class SaleItemRepository : ISaleItemRepository
    {
        private readonly DefaultContext _context;

        public SaleItemRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<SaleItem> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.SaleItems.FirstOrDefaultAsync(si => si.Id == id, cancellationToken);
        }

        public async Task<SaleItem> CreateAsync(SaleItem saleItem, CancellationToken cancellationToken)
        {
            _context.SaleItems.Add(saleItem);
            await _context.SaveChangesAsync(cancellationToken);
            return saleItem;
        }
    }
}
