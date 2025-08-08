using AutoMapper;
using MediatR;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using DotNetCore.EnterpriseTemplate.Domain.Entities;

namespace DotNetCore.EnterpriseTemplate.Application.Sales.DeleteSale;

public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, DeleteSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public DeleteSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<DeleteSaleResult> Handle(DeleteSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {command.Id} not found");

        if (sale.IsActive)
             throw new InvalidOperationException("Cannot delete an active sale. Cancel it first.");

        await _saleRepository.DeleteAsync(sale.Id, cancellationToken);

        return _mapper.Map<DeleteSaleResult>(sale);
    }
}
