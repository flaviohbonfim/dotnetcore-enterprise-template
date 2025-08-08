using DotNetCore.EnterpriseTemplate.Application.Sales.CancelSale;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Application;

public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid sale id When canceling sale Then returns success response")]
    public async Task Handle_ValidSaleId_ReturnsCancelledSale()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand { Id = saleId };
        var sale = new Sale { Id = saleId, IsCancelled = false };
        var expectedResult = new CancelSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<CancelSaleResult>(sale)
            .Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.IsCancelled == true),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent sale id When canceling Then throws InvalidOperationException")]
    public async Task Handle_NonExistentSaleId_ThrowsInvalidOperationException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    [Fact(DisplayName = "Given already cancelled sale When canceling Then throws InvalidOperationException")]
    public async Task Handle_AlreadyCancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new CancelSaleCommand { Id = saleId };
        var sale = new Sale { Id = saleId, IsCancelled = true };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale is already cancelled");
    }
}