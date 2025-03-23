using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale
{
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, Unit>
    {
        private readonly ISaleRepository _saleRepository;

        public DeleteSaleHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        public async Task<Unit> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            // Obtém a venda no repositório
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            
            if (sale == null)
            {
                // Lançar exceção ou retornar erro, dependendo da necessidade
                throw new Exception("Sale not found");
            }

            // Exclui a venda
            await _saleRepository.DeleteAsync(sale.Id, cancellationToken);

            // Retorna Unit.Value, que é o valor para indicar sucesso sem retornar dados
            return Unit.Value;
        }
    }
}
