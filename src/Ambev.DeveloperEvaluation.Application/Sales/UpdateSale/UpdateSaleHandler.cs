using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale
{
    public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResponse>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<UpdateSaleResponse> Handle(UpdateSaleCommand request, CancellationToken cancellationToken)
        {
            var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            if (sale == null)
            {
                throw new Exception("Sale not found");
            }

            // Atualiza os campos da venda
            sale.SaleNumber = request.SaleNumber;
            sale.SaleDate = request.SaleDate;
            sale.TotalAmount = request.TotalAmount;
            sale.Items = _mapper.Map<List<SaleItem>>(request.Items);

            await _saleRepository.UpdateAsync(sale, cancellationToken);

            return _mapper.Map<UpdateSaleResponse>(sale);
        }
    }
}
