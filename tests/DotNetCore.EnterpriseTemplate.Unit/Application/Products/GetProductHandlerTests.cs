using DotNetCore.EnterpriseTemplate.Application.Products.GetProduct;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Application.Products;

public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetProductHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid id When getting product Then returns product details")]
    public async Task Handle_ValidId_ReturnsProduct()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new GetProductCommand { Id = productId };
        var product = new Product { Id = productId, Name = "Test Product" };
        var expectedResult = new GetProductResult { Id = productId, Name = "Test Product" };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns(product);
        _mapper.Map<GetProductResult>(product).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be("Test Product");
    }

    [Fact(DisplayName = "Given non-existent id When getting product Then throws InvalidOperationException")]
    public async Task Handle_NonExistentId_ThrowsInvalidOperationException()
    {
        // Given
        var productId = Guid.NewGuid();
        var command = new GetProductCommand { Id = productId };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>())
            .Returns((Product)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Product with ID {productId} not found");
    }
}