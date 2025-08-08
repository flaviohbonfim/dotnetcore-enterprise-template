using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using AutoMapper;
using MediatR;
using FluentValidation;
using FluentValidation.Results;

namespace DotNetCore.EnterpriseTemplate.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        ICustomerRepository customerRepository,
        IBranchRepository branchRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _customerRepository = customerRepository;
        _branchRepository = branchRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        // Validate if customer exists
        var customer = await _customerRepository.GetByIdAsync((Guid)command.CustomerId, cancellationToken);
        if (customer == null)
            throw new InvalidOperationException($"Customer with ID {command.CustomerId} not found");

        // Validate if branch exists
        var branch = await _branchRepository.GetByIdAsync((Guid)command.BranchId, cancellationToken);
        if (branch == null)
            throw new InvalidOperationException($"Branch with ID {command.BranchId} not found");

        // Validate if sale number is unique
        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        // Validate item quantities and calculate discounts
        foreach (var item in command.Items)
        {
            // Validate maximum quantity
            if (item.Quantity > 20)
                throw new ValidationException(new[] 
                { 
                    new ValidationFailure("Quantity", "Cannot sell more than 20 units of the same product")
                });

            // Calculate discount based on quantity
            decimal discountPercentage = 0;
            if (item.Quantity >= 10)
                discountPercentage = 0.20m; // 20% discount
            else if (item.Quantity >= 4)
                discountPercentage = 0.10m; // 10% discount

            // Apply discount
            decimal itemTotal = item.Quantity * item.UnitPrice;
            item.Discount = discountPercentage > 0 ? itemTotal * discountPercentage : 0;
        }

        var sale = _mapper.Map<Sale>(command);
        
        // Validate and set products
        foreach (var item in command.Items)
        {
            var product = await _productRepository.GetByIdAsync((Guid)item.ProductId, cancellationToken);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
        }

        // Calculate total amount with discounts
        sale.TotalAmount = command.Items.Sum(item => (item.Quantity * item.UnitPrice) - item.Discount);

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);
        
        return _mapper.Map<CreateSaleResult>(createdSale);
    }
}
