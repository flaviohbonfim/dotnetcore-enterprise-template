using DotNetCore.EnterpriseTemplate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCore.EnterpriseTemplate.Domain.Repositories
{
    public interface ISaleItemRepository
    {
        Task<SaleItem> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<SaleItem> CreateAsync(SaleItem saleItem, CancellationToken cancellationToken);
    }
}