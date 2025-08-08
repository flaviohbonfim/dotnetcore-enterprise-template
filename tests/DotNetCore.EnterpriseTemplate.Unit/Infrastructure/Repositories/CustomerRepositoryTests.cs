using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.ORM;
using DotNetCore.EnterpriseTemplate.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Infrastructure.Repositories;

public class CustomerRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly CustomerRepository _repository;

    public CustomerRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new CustomerRepository(_context);
    }

    [Fact(DisplayName = "Given valid customer When creating Then returns created customer")]
    public async Task CreateAsync_ValidCustomer_ReturnsCreatedCustomer()
    {
        // Given
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St"
        };

        // When
        var result = await _repository.CreateAsync(customer, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(customer.Name);
        result.Email.Should().Be(customer.Email);

        var savedCustomer = await _context.Customers.FindAsync(result.Id);
        savedCustomer.Should().NotBeNull();
        savedCustomer.Name.Should().Be(customer.Name);
    }

    [Fact(DisplayName = "Given existing customer When getting by id Then returns customer")]
    public async Task GetByIdAsync_ExistingCustomer_ReturnsCustomer()
    {
        // Given
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // When
        var result = await _repository.GetByIdAsync(customer.Id, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
        result.Name.Should().Be(customer.Name);
    }

    [Fact(DisplayName = "Given non-existent customer When getting by id Then returns null")]
    public async Task GetByIdAsync_NonExistentCustomer_ReturnsNull()
    {
        // When
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Then
        result.Should().BeNull();
    }

    [Fact(DisplayName = "Given existing customer When getting by email Then returns customer")]
    public async Task GetByEmailAsync_ExistingEmail_ReturnsCustomer()
    {
        // Given
        var email = "john@example.com";
        var customer = new Customer
        {
            Name = "John Doe",
            Email = email
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        // When
        var result = await _repository.GetByEmailAsync(email, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Email.Should().Be(email);
    }

    [Fact(DisplayName = "Given active customers When getting all Then returns ordered list")]
    public async Task GetAllAsync_ActiveCustomers_ReturnsOrderedList()
    {
        // Given
        var customers = new[]
        {
            new Customer { Name = "John Doe", IsActive = true },
            new Customer { Name = "Alice Smith", IsActive = true },
            new Customer { Name = "Bob Johnson", IsActive = false }
        };
        _context.Customers.AddRange(customers);
        await _context.SaveChangesAsync();

        // When
        var result = await _repository.GetAllAsync(CancellationToken.None);

        // Then
        result.Should().HaveCount(2); // Only active customers
        result.Should().BeInAscendingOrder(x => x.Name);
        result.Should().NotContain(x => !x.IsActive);
    }

    [Fact(DisplayName = "Given existing customer When updating Then returns updated customer")]
    public async Task UpdateAsync_ExistingCustomer_ReturnsUpdatedCustomer()
    {
        // Given
        var customer = new Customer
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        customer.Name = "John Doe Updated";
        customer.Email = "john.updated@example.com";

        // When
        var result = await _repository.UpdateAsync(customer, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Name.Should().Be("John Doe Updated");
        result.Email.Should().Be("john.updated@example.com");

        var updatedCustomer = await _context.Customers.FindAsync(customer.Id);
        updatedCustomer.Should().NotBeNull();
        updatedCustomer.Name.Should().Be("John Doe Updated");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}