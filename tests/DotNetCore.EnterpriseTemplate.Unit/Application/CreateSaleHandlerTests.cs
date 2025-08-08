using DotNetCore.EnterpriseTemplate.Application.Sales.CreateSale;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using DotNetCore.EnterpriseTemplate.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Application;

public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IBranchRepository _branchRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _customerRepository = Substitute.For<ICustomerRepository>();
        _branchRepository = Substitute.For<IBranchRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateSaleHandler(
            _saleRepository,
            _customerRepository,
            _branchRepository,
            _productRepository,
            _mapper);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var customer = new Customer { Id = command.CustomerId.Value };
        var branch = new Branch { Id = command.BranchId.Value };
        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            CustomerId = command.CustomerId.Value,
            BranchId = command.BranchId.Value,
            SaleNumber = command.SaleNumber,
            TotalAmount = command.Items.Sum(i => i.Quantity * i.UnitPrice)
        };

        _customerRepository.GetByIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns(customer);
        _branchRepository.GetByIdAsync(command.BranchId.Value, Arg.Any<CancellationToken>())
            .Returns(branch);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        foreach (var item in command.Items)
        {
            var product = new Product { Id = item.ProductId.Value };
            _productRepository.GetByIdAsync(item.ProductId.Value, Arg.Any<CancellationToken>())
                .Returns(product);
        }

        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CreateSaleResult>(sale).Returns(new CreateSaleResult { Id = sale.Id });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent customer When creating sale Then throws InvalidOperationException")]
    public async Task Handle_NonExistentCustomer_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        _customerRepository.GetByIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns((Customer)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Customer with ID {command.CustomerId} not found");
    }

    [Fact(DisplayName = "Given non-existent branch When creating sale Then throws InvalidOperationException")]
    public async Task Handle_NonExistentBranch_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var customer = new Customer { Id = command.CustomerId.Value };

        _customerRepository.GetByIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns(customer);
        _branchRepository.GetByIdAsync(command.BranchId.Value, Arg.Any<CancellationToken>())
            .Returns((Branch)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Branch with ID {command.BranchId} not found");
    }

    [Fact(DisplayName = "Given duplicate sale number When creating sale Then throws InvalidOperationException")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var customer = new Customer { Id = command.CustomerId.Value };
        var branch = new Branch { Id = command.BranchId.Value };
        var existingSale = new Sale { SaleNumber = command.SaleNumber };

        _customerRepository.GetByIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns(customer);
        _branchRepository.GetByIdAsync(command.BranchId.Value, Arg.Any<CancellationToken>())
            .Returns(branch);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    [Fact(DisplayName = "Given item quantity exceeding limit When creating sale Then throws ValidationException")]
    public async Task Handle_ItemQuantityExceedsLimit_ThrowsValidationException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        command.Items.First().Quantity = 21; // Exceeds limit of 20

        var customer = new Customer { Id = command.CustomerId.Value };
        var branch = new Branch { Id = command.BranchId.Value };

        _customerRepository.GetByIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns(customer);
        _branchRepository.GetByIdAsync(command.BranchId.Value, Arg.Any<CancellationToken>())
            .Returns(branch);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Cannot sell more than 20 units of the same product*");
    }

    [Theory(DisplayName = "Given different quantities When creating sale Then applies correct discount")]
    [InlineData(3, 0)] // No discount for less than 4 items
    [InlineData(4, 0.10)] // 10% discount for 4-9 items
    [InlineData(9, 0.10)] // 10% discount for 4-9 items
    [InlineData(10, 0.20)] // 20% discount for 10+ items
    [InlineData(15, 0.20)] // 20% discount for 10+ items
    public async Task Handle_DifferentQuantities_AppliesCorrectDiscount(int quantity, decimal expectedDiscountPercentage)
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        command.Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = quantity,
                UnitPrice = 100m // Using 100 for easy percentage calculation
            }
        };
    
        var customer = new Customer { Id = command.CustomerId.Value };
        var branch = new Branch { Id = command.BranchId.Value };
        var product = new Product { Id = command.Items[0].ProductId.Value };
    
        _customerRepository.GetByIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns(customer);
        _branchRepository.GetByIdAsync(command.BranchId.Value, Arg.Any<CancellationToken>())
            .Returns(branch);
        _productRepository.GetByIdAsync(command.Items[0].ProductId.Value, Arg.Any<CancellationToken>())
            .Returns(product);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
    
        var sale = new Sale { Id = Guid.NewGuid() };
        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
    
        // When
        await _handler.Handle(command, CancellationToken.None);
    
        // Then
        var expectedDiscount = quantity * 100m * expectedDiscountPercentage;
        command.Items[0].Discount.Should().Be(expectedDiscount);
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.TotalAmount == (quantity * 100m - expectedDiscount)),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given multiple items When creating sale Then applies correct discount for each item")]
    public async Task Handle_MultipleItems_AppliesCorrectDiscountForEachItem()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        command.Items = new List<CreateSaleItemCommand>
        {
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = 3,  // No discount
                UnitPrice = 100m
            },
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = 5,  // 10% discount
                UnitPrice = 100m
            },
            new CreateSaleItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = 12, // 20% discount
                UnitPrice = 100m
            }
        };
    
        var customer = new Customer { Id = command.CustomerId.Value };
        var branch = new Branch { Id = command.BranchId.Value };
    
        _customerRepository.GetByIdAsync(command.CustomerId.Value, Arg.Any<CancellationToken>())
            .Returns(customer);
        _branchRepository.GetByIdAsync(command.BranchId.Value, Arg.Any<CancellationToken>())
            .Returns(branch);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
    
        foreach (var item in command.Items)
        {
            var product = new Product { Id = item.ProductId.Value };
            _productRepository.GetByIdAsync(item.ProductId.Value, Arg.Any<CancellationToken>())
                .Returns(product);
        }
    
        var sale = new Sale { Id = Guid.NewGuid() };
        _mapper.Map<Sale>(command).Returns(sale);
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);
    
        // When
        await _handler.Handle(command, CancellationToken.None);
    
        // Then
        command.Items[0].Discount.Should().Be(0m);                    // No discount
        command.Items[1].Discount.Should().Be(500m * 0.10m);         // 10% of 500
        command.Items[2].Discount.Should().Be(1200m * 0.20m);        // 20% of 1200
    
        var expectedTotal = (300m) +                  // First item: 3 * 100 - 0
                       (500m - 50m) +             // Second item: 5 * 100 - (500 * 0.10)
                       (1200m - 240m);            // Third item: 12 * 100 - (1200 * 0.20)
    
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s => s.TotalAmount == expectedTotal),
            Arg.Any<CancellationToken>());
    }
}