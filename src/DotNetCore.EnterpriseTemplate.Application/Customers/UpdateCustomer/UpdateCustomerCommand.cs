using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Customers.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<UpdateCustomerResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}