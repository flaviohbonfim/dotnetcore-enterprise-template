using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Customers.GetCustomers;

public class GetCustomersCommand : IRequest<List<GetCustomersResult>>
{
}