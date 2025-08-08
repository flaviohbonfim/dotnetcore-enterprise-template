using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore.EnterpriseTemplate.ORM.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly DefaultContext _context;

        public SaleRepository(DefaultContext context)
        {
            _context = context;
        }

        // Método para criar uma nova venda
        public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken)
        {
            _context.Entry(sale).State = EntityState.Added;
            
            foreach (var item in sale.Items)
            {
                _context.Entry(item).State = EntityState.Added;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return sale;
        }
        public async Task<Sale> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
        }

        // Método para obter uma venda pelo ID, incluindo os itens e produtos
        public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Branch)        // Inclui o branch da venda
                .Include(s => s.Customer)      // Inclui o cliente da venda
                .Include(s => s.Items)         // Inclui os itens da venda
                .ThenInclude(si => si.Product) // Inclui os produtos dentro dos itens
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        // Método para obter todas as vendas (opcional)
        public async Task<IEnumerable<Sale>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Sales
                .Include(s => s.Branch)        // Inclui o branch da venda
                .Include(s => s.Customer)      // Inclui o cliente da venda
                .Include(s => s.Items)         // Inclui os itens da venda
                .ThenInclude(si => si.Product) // Inclui os produtos dentro dos itens
                .ToListAsync(cancellationToken); // Converte para List e retorna como IEnumerable
        }

        // Método para excluir uma venda
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var sale = await GetByIdAsync(id, cancellationToken);
            if (sale == null) return false;
            sale.IsActive = false;

            _context.Sales.Update(sale);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        // Método para atualizar a venda, se necessário
        public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync(cancellationToken);
            return sale;
        }
    }
}
