using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResponse>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IMapper _mapper;

        public CreateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
        {
            _saleRepository = saleRepository;
            _mapper = mapper;
        }

        public async Task<CreateSaleResponse> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
        {
            // Mapeia a criação de uma venda
            var sale = _mapper.Map<Sale>(request);
            
            // Adiciona os itens da venda
            foreach (var itemDto in request.Items)
            {
                var item = _mapper.Map<SaleItem>(itemDto);
                sale.Items.Add(item);
            }

            // Salva a venda no repositório
            await _saleRepository.CreateAsync(sale, cancellationToken);

            // Retorna a resposta
            return _mapper.Map<CreateSaleResponse>(sale);
        }
    }
}
