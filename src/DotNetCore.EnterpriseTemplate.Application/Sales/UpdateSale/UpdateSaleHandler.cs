using AutoMapper;
using MediatR;
using FluentValidation;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using DotNetCore.EnterpriseTemplate.Domain.Entities;

namespace DotNetCore.EnterpriseTemplate.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
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

        var existingSaleWithNumber = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSaleWithNumber != null && existingSaleWithNumber.Id != command.SaleId)
            throw new InvalidOperationException($"Sale number {command.SaleNumber} is already in use");

        // Validate products exist
        foreach (var item in command.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
        }

        _mapper.Map(command, sale);
        
        // Calculate total amount
        sale.TotalAmount = command.Items.Sum(item => (item.Quantity * item.UnitPrice) - item.Discount);

        var updatedSale = await _saleRepository.UpdateAsync(sale, cancellationToken);
        
        return _mapper.Map<UpdateSaleResult>(updatedSale);
    }
}
