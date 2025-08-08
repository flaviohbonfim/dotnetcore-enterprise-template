using AutoMapper;
using MediatR;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;
using FluentValidation;

namespace DotNetCore.EnterpriseTemplate.Application.Customers.CreateCustomer;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResult>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CreateCustomerHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateCustomerCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingCustomer = await _customerRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingCustomer != null)
            throw new InvalidOperationException($"Customer with email {command.Email} already exists");

        var customer = _mapper.Map<Customer>(command);
        var createdCustomer = await _customerRepository.CreateAsync(customer, cancellationToken);
        
        return _mapper.Map<CreateCustomerResult>(createdCustomer);
    }
}