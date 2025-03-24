using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.SaleId, cancellationToken);
        if (sale == null)
            throw new InvalidOperationException($"Sale with ID {command.SaleId} not found");

        _mapper.Map(command, sale);
        
        // Calculate total amount
        sale.TotalAmount = command.Items.Sum(item => (item.Quantity * item.UnitPrice) - item.Discount);

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        
        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
