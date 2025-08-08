using Ambev.DeveloperEvaluation.Application.Branches.CreateBranch;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Branches;

public class CreateBranchHandlerTests
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly CreateBranchHandler _handler;

    public CreateBranchHandlerTests()
    {
        _branchRepository = Substitute.For<IBranchRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateBranchHandler(_branchRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid branch data When creating branch Then returns success")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Given
        var command = new CreateBranchCommand
        {
            Name = "Test Branch",
            Address = "Test Address",
            Phone = "1234567890"
        };

        var branch = new Branch { Id = Guid.NewGuid(), Name = command.Name };
        var expectedResult = new CreateBranchResult { Id = branch.Id };

        _mapper.Map<Branch>(command).Returns(branch);
        _branchRepository.CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>())
            .Returns(branch);
        _mapper.Map<CreateBranchResult>(branch).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(branch.Id);
        await _branchRepository.Received(1).CreateAsync(Arg.Any<Branch>(), Arg.Any<CancellationToken>());
    }

    // [Fact(DisplayName = "Given duplicate branch name When creating Then throws InvalidOperationException")]
    // public async Task Handle_DuplicateName_ThrowsInvalidOperationException()
    // {
    //     // Given
    //     var command = new CreateBranchCommand { Name = "Existing Branch" };
    //     var existingBranch = new Branch { Name = "Existing Branch" };

    //     _branchRepository.GetByNameAsync(command.Name, Arg.Any<CancellationToken>())
    //         .Returns(existingBranch);

    //     // When
    //     var act = () => _handler.Handle(command, CancellationToken.None);

    //     // Then
    //     await act.Should().ThrowAsync<InvalidOperationException>()
    //         .WithMessage($"Branch with name {command.Name} already exists");
    // }
}