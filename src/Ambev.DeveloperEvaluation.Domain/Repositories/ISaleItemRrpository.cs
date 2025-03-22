using Ambev.DeveloperEvaluation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Domain.Repositories
{
    public interface ISaleItemRepository
    {
        Task<SaleItem> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<SaleItem> CreateAsync(SaleItem saleItem, CancellationToken cancellationToken);
    }
}