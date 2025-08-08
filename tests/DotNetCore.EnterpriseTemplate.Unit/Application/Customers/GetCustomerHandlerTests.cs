using DotNetCore.EnterpriseTemplate.Application.Customers.GetCustomer;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace DotNetCore.EnterpriseTemplate.Unit.Application.Customers;

public class GetCustomerHandlerTests
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly GetCustomerHandler _handler;

    public GetCustomerHandlerTests()
    {
        _customerRepository = Substitute.For<ICustomerRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCustomerHandler(_customerRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid id When getting customer Then returns customer details")]
    public async Task Handle_ValidId_ReturnsCustomer()
    {
        // Given
        var customerId = Guid.NewGuid();
        var command = new GetCustomerCommand { Id = customerId };
        var customer = new Customer 
        { 
            Id = customerId, 
            Name = "John Doe",
            Email = "john@example.com"
        };
        var expectedResult = new GetCustomerResult 
        { 
            Id = customerId,
            Name = "John Doe",
            Email = "john@example.com"
        };

        _customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns(customer);
        _mapper.Map<GetCustomerResult>(customer).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(customerId);
        result.Name.Should().Be("John Doe");
        result.Email.Should().Be("john@example.com");
    }

    [Fact(DisplayName = "Given non-existent id When getting customer Then throws InvalidOperationException")]
    public async Task Handle_NonExistentId_ThrowsInvalidOperationException()
    {
        // Given
        var customerId = Guid.NewGuid();
        var command = new GetCustomerCommand { Id = customerId };

        _customerRepository.GetByIdAsync(customerId, Arg.Any<CancellationToken>())
            .Returns((Customer)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Customer with ID {customerId} not found");
    }
}