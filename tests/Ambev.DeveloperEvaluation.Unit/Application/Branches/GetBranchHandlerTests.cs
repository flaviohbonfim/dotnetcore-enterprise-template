using Ambev.DeveloperEvaluation.Application.Branches.GetBranch;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Branches;

public class GetBranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly GetBranchHandler _handler;

    public GetBranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetBranchHandler(_branchRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid id When getting branch Then returns branch details")]
    public async Task Handle_ValidId_ReturnsBranch()
    {
        // Given
        var branchId = Guid.NewGuid();
        var command = new GetBranchCommand { Id = branchId };
        var branch = new Branch { Id = branchId, Name = "Test Branch" };
        var expectedResult = new GetBranchResult { Id = branchId, Name = "Test Branch" };

        _branchRepository.GetByIdAsync(branchId, Arg.Any<CancellationToken>())
            .Returns(branch);
        _mapper.Map<GetBranchResult>(branch).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(branchId);
        result.Name.Should().Be("Test Branch");
    }

    [Fact(DisplayName = "Given non-existent id When getting branch Then throws InvalidOperationException")]
    public async Task Handle_NonExistentId_ThrowsInvalidOperationException()
    {
        // Given
        var branchId = Guid.NewGuid();
        var command = new GetBranchCommand { Id = branchId };

        _branchRepository.GetByIdAsync(branchId, Arg.Any<CancellationToken>())
            .Returns((Branch)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Branch with ID {branchId} not found");
    }
}