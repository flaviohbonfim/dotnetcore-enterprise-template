using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _productRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid update data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            SaleId = saleId,
            SaleNumber = "SALE001",
            SaleDate = DateTime.Now,
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 100m }
            }
        };

        var existingSale = new Sale { Id = saleId };
        var updatedSale = new Sale { Id = saleId };
        var expectedResult = new UpdateSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
            
        _mapper.Map(command, existingSale).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(expectedResult);

        foreach (var item in command.Items)
        {
            var product = new Product { Id = item.ProductId };
            _productRepository.GetByIdAsync(item.ProductId, Arg.Any<CancellationToken>())
                .Returns(product);
        }

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale When updating Then throws InvalidOperationException")]
    public async Task Handle_NonExistentSale_ThrowsInvalidOperationException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand 
        { 
            SaleId = saleId,
            SaleNumber = "SALE001",
            SaleDate = DateTime.Now,
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 100m }
            }
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    [Fact(DisplayName = "Given duplicate sale number When updating Then throws InvalidOperationException")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new UpdateSaleCommand
        {
            SaleId = saleId,
            SaleNumber = "SALE001",
            SaleDate = DateTime.Now,
            Items = new List<UpdateSaleItemCommand>
            {
                new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 100m }
            }
        };

        var existingSale = new Sale { Id = saleId };
        var duplicateSale = new Sale { Id = Guid.NewGuid(), SaleNumber = "SALE001" };
        
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(duplicateSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale number {command.SaleNumber} is already in use");
    }
}