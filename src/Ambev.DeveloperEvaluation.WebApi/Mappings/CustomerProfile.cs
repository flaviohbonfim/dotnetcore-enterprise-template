using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Application.Customers.GetCustomer;
using Ambev.DeveloperEvaluation.Application.Customers.GetCustomers;
using Ambev.DeveloperEvaluation.Application.Customers.CreateCustomer;
using Ambev.DeveloperEvaluation.WebApi.Features.Customers.CreateCustomer;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<Customer, GetCustomerResult>();
        CreateMap<Customer, GetCustomersResult>();
        CreateMap<CreateCustomerRequest, CreateCustomerCommand>();
        CreateMap<Customer, CreateCustomerResult>();
    }
}