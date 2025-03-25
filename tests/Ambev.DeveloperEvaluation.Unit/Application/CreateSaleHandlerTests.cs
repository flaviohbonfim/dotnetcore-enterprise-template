using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

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
}