using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Customers.GetCustomer;

public class GetCustomerCommand : IRequest<GetCustomerResult>
{
    public Guid Id { get; set; }
}