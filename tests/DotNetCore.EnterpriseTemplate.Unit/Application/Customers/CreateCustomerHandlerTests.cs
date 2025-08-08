using DotNetCore.EnterpriseTemplate.Application.Customers.CreateCustomer;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Application.Customers;

public class CreateCustomerHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly CreateCustomerHandler _handler;

    public CreateCustomerHandlerTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateCustomerHandler(_customerRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid customer data When creating customer Then returns success")]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        // Given
        var command = new CreateCustomerCommand
        {
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "1234567890",
            Address = "123 Main St"
        };

        var customer = new Customer 
        { 
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email
        };
        var expectedResult = new CreateCustomerResult { Id = customer.Id };

        _customerRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns((Customer)null);
        _mapper.Map<Customer>(command).Returns(customer);
        _customerRepository.CreateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>())
            .Returns(customer);
        _mapper.Map<CreateCustomerResult>(customer).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(customer.Id);
        await _customerRepository.Received(1).CreateAsync(Arg.Any<Customer>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given duplicate email When creating Then throws InvalidOperationException")]
    public async Task Handle_DuplicateEmail_ThrowsInvalidOperationException()
    {
        // Given
        var command = new CreateCustomerCommand
        {
            Name = "John Doe",
            Email = "existing@example.com",
            Phone = "1234567890",
            Address = "123 Main St"
        };

        var existingCustomer = new Customer 
        { 
            Id = Guid.NewGuid(),
            Email = command.Email 
        };

        _customerRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>())
            .Returns(existingCustomer);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Customer with email {command.Email} already exists");
    }

    [Fact(DisplayName = "Given invalid data When creating Then throws ValidationException")]
    public async Task Handle_InvalidData_ThrowsValidationException()
    {
        // Given
        var command = new CreateCustomerCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}