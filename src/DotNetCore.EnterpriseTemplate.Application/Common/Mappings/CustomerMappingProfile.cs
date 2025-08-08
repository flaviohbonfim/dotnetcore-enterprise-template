using AutoMapper;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Application.Customers.UpdateCustomer;

namespace DotNetCore.EnterpriseTemplate.Application.Common.Mappings;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<UpdateCustomerCommand, Customer>();
        CreateMap<Customer, UpdateCustomerResult>();
    }
}