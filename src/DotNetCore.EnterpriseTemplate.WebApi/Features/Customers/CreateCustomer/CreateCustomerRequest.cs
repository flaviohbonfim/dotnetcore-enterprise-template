using System.ComponentModel.DataAnnotations;

namespace DotNetCore.EnterpriseTemplate.WebApi.Features.Customers.CreateCustomer;

public class CreateCustomerRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(14)]
    public string Document { get; set; }

    [Required]
    [StringLength(200)]
    public string Address { get; set; }

    [Required]
    [StringLength(20)]
    public string Phone { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
}