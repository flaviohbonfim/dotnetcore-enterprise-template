using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.ORM;
using DotNetCore.EnterpriseTemplate.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Infrastructure.Repositories;

public class SaleRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly SaleRepository _repository;

    public SaleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new SaleRepository(_context);
    }

    [Fact(DisplayName = "Given valid sale When creating Then returns created sale")]
    public async Task CreateAsync_ValidSale_ReturnsCreatedSale()
    {
        // Given
        var customer = new Customer { Name = "Test Customer" };
        var branch = new Branch { Name = "Test Branch" };
        var product = new Product { Name = "Test Product", Price = 10.99m };

        _context.Customers.Add(customer);
        _context.Branches.Add(branch);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var sale = new Sale
        {
            CustomerId = customer.Id,
            BranchId = branch.Id,
            SaleDate = DateTime.UtcNow,
            Items = new List<SaleItem>
            {
                new() { ProductId = product.Id, Quantity = 2, UnitPrice = product.Price }
            }
        };

        // When
        var result = await _repository.CreateAsync(sale, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CustomerId.Should().Be(customer.Id);
        result.Items.Should().HaveCount(1);

        var savedSale = await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == result.Id);
        savedSale.Should().NotBeNull();
        savedSale.Items.Should().HaveCount(1);
    }
    [Fact(DisplayName = "Given existing sale When getting by id Then returns sale with items")]
    public async Task GetByIdAsync_ExistingSale_ReturnsSaleWithItems()
    {
        // Arrange
        var customer = new Customer 
        { 
            Id = Guid.NewGuid(),
            Name = "Test Customer",
            IsActive = true,
            Email = "test@customer.com"
        };

        var branch = new Branch 
        { 
            Id = Guid.NewGuid(),
            Name = "Test Branch",
            IsActive = true,
            Address = "Test Address",
            Phone = "1234567890"
        };

        var product = new Product 
        { 
            Id = Guid.NewGuid(),
            Name = "Test Product", 
            Price = 10.99m,
            IsActive = true
        };

        await _context.Database.EnsureDeletedAsync();

        _context.Customers.Add(customer);
        _context.Branches.Add(branch);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var saleId = Guid.NewGuid();
        var saleItemId = Guid.NewGuid();

        var sale = new Sale
        {
            Id = saleId,
            CustomerId = customer.Id,
            BranchId = branch.Id,
            SaleDate = DateTime.UtcNow
        };

        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var saleItem = new SaleItem
        { 
            Id = saleItemId,
            SaleId = saleId,
            ProductId = product.Id,
            Quantity = 2,
            UnitPrice = product.Price
        };

        _context.SaleItems.Add(saleItem);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetByIdAsync(saleId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(saleId);
        result.CustomerId.Should().Be(customer.Id);
        result.BranchId.Should().Be(branch.Id);
        result.Items.Should().HaveCount(1);

        var resultItem = result.Items.First();
        resultItem.Id.Should().Be(saleItemId);
        resultItem.ProductId.Should().Be(product.Id);
        resultItem.Quantity.Should().Be(2);
        resultItem.UnitPrice.Should().Be(product.Price);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}