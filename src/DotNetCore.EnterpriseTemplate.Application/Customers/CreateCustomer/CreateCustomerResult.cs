namespace DotNetCore.EnterpriseTemplate.Application.Customers.CreateCustomer;

public class CreateCustomerResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}