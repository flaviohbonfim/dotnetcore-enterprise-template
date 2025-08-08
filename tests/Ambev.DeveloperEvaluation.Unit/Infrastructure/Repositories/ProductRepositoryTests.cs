using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Infrastructure.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact(DisplayName = "Given valid product When creating Then returns created product")]
    public async Task CreateAsync_ValidProduct_ReturnsCreatedProduct()
    {
        // Given
        var product = new Product
        {
            Name = "Test Product",
            Price = 10.99m,
            IsActive = true
        };

        // When
        var result = await _repository.CreateAsync(product, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(product.Name);
        result.Price.Should().Be(product.Price);

        var savedProduct = await _context.Products.FindAsync(result.Id);
        savedProduct.Should().NotBeNull();
        savedProduct.Name.Should().Be(product.Name);
    }

    [Fact(DisplayName = "Given existing product When getting by id Then returns product")]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
    {
        // Given
        var product = new Product
        {
            Name = "Test Product",
            Price = 10.99m
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // When
        var result = await _repository.GetByIdAsync(product.Id, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(product.Id);
        result.Name.Should().Be(product.Name);
    }

    [Fact(DisplayName = "Given active products When getting all Then returns ordered list")]
    public async Task GetAllAsync_ActiveProducts_ReturnsOrderedList()
    {
        // Given
        var products = new[]
        {
            new Product { Name = "Product B", IsActive = true, Price = 10.99m },
            new Product { Name = "Product A", IsActive = true, Price = 20.99m },
            new Product { Name = "Product C", IsActive = false, Price = 30.99m }
        };
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // When
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Then
        result.Should().HaveCount(2);
        result.Should().BeInAscendingOrder(x => x.Name);
        result.Should().NotContain(x => !x.IsActive);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}