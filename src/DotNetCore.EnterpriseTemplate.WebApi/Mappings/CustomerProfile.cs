using AutoMapper;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Application.Customers.GetCustomer;
using DotNetCore.EnterpriseTemplate.Application.Customers.GetCustomers;
using DotNetCore.EnterpriseTemplate.Application.Customers.CreateCustomer;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Customers.CreateCustomer;

namespace DotNetCore.EnterpriseTemplate.WebApi.Mappings;

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