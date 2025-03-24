using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Customers.GetCustomers;

public class GetCustomersCommand : IRequest<List<GetCustomersResult>>
{
}