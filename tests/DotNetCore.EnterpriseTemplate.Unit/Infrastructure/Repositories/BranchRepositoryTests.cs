using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.ORM;
using DotNetCore.EnterpriseTemplate.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Infrastructure.Repositories;

public class BranchRepositoryTests : IDisposable
{
    private readonly DefaultContext _context;
    private readonly BranchRepository _repository;

    public BranchRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DefaultContext(options);
        _repository = new BranchRepository(_context);
    }

    [Fact(DisplayName = "Given valid branch When creating Then returns created branch")]
    public async Task CreateAsync_ValidBranch_ReturnsCreatedBranch()
    {
        // Given
        var branch = new Branch
        {
            Name = "Test Branch",
            Address = "123 Test St",
            Phone = "1234567890"
        };

        // When
        var result = await _repository.CreateAsync(branch, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Name.Should().Be(branch.Name);

        var savedBranch = await _context.Branches.FindAsync(result.Id);
        savedBranch.Should().NotBeNull();
        savedBranch.Name.Should().Be(branch.Name);
    }

    [Fact(DisplayName = "Given existing branch When getting by id Then returns branch")]
    public async Task GetByIdAsync_ExistingBranch_ReturnsBranch()
    {
        // Given
        var branch = new Branch
        {
            Name = "Test Branch",
            Address = "123 Test St"
        };
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();

        // When
        var result = await _repository.GetByIdAsync(branch.Id, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(branch.Id);
        result.Name.Should().Be(branch.Name);
    }

    [Fact(DisplayName = "Given active branches When getting all Then returns ordered list")]
    public async Task GetAllAsync_ActiveBranches_ReturnsOrderedList()
    {
        // Given
        var branches = new[]
        {
            new Branch { Name = "Branch B", IsActive = true },
            new Branch { Name = "Branch A", IsActive = true },
            new Branch { Name = "Branch C", IsActive = false }
        };
        _context.Branches.AddRange(branches);
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