using Ambev.DeveloperEvaluation.Application.Customers.UpdateCustomer;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Customers;

public class UpdateCustomerHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly UpdateCustomerHandler _handler;

    public UpdateCustomerHandlerTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateCustomerHandler(_customerRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid update data When updating customer Then returns success")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Given
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand
        {
            Id = customerId,
            Name = "John Doe Updated",
            Email = "john.updated@example.com",
            Phone = "0987654321",
            Address = "456 New St"
        };

        var existingCustomer = new Customer { Id = customerId };
        var updatedCustomer = new Customer { Id = customerId };
        var expectedResult = new UpdateCustomerResult { Id = customerId };

        _customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(existingCustomer);
        _customerRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((Customer)null);
        _mapper.Map(command, existingCustomer).Returns(updatedCustomer);
        _customerRepository.UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(updatedCustomer);
        _mapper.Map<UpdateCustomerResult>(updatedCustomer).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(customerId);
        await _customerRepository.Received(1).UpdateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existent customer When updating Then throws InvalidOperationException")]
    public async Task Handle_NonExistentCustomer_ThrowsInvalidOperationException()
    {
        // Given
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand 
        { 
            Id = customerId,
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St"
        };

        _customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns((Customer)null);
        _customerRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((Customer)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Customer with ID {customerId} not found");
    }

    [Fact(DisplayName = "Given duplicate email When updating Then throws InvalidOperationException")]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Given
        var customerId = Guid.NewGuid();
        var command = new UpdateCustomerCommand
        {
            Id = customerId,
            Name = "John Doe",
            Email = "existing@example.com",
            Phone = "1234567890",
            Address = "123 Main St"
        };

        var existingCustomer = new Customer { Id = customerId };
        var duplicateCustomer = new Customer 
        { 
            Id = Guid.NewGuid(), 
            Email = command.Email 
        };

        _customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(existingCustomer);
        _customerRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(duplicateCustomer);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Email {command.Email} is already in use by another customer");
    }
}