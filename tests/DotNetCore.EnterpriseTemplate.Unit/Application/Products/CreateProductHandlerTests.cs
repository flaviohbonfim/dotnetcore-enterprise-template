using DotNetCore.EnterpriseTemplate.Application.Products.CreateProduct;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Application.Products;

public class CreateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateProductHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid product data When creating product Then returns success")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Given
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 100m
        };

        var product = new Product { Id = Guid.NewGuid(), Name = command.Name };
        var expectedResult = new CreateProductResult { Id = product.Id };

        _mapper.Map<Product>(command).Returns(product);
        _productRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(product);
        _mapper.Map<CreateProductResult>(product).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        await _productRepository.Received(1).CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    // [Fact(DisplayName = "Given duplicate product name When creating Then throws InvalidOperationException")]
    // public async Task Handle_DuplicateName_ThrowsInvalidOperationException()
    // {
    //     // Given
    //     var command = new CreateProductCommand { Name = "Existing Product" };
    //     var existingProduct = new Product { Name = "Existing Product" };

    //     _productRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
    //         .Returns(existingProduct);

    //     // When
    //     var act = () => _handler.Handle(command, CancellationToken.None);

    //     // Then
    //     await act.Should().ThrowAsync<InvalidOperationException>()
    //         .WithMessage($"Product with name {command.Name} already exists");
    // }
}