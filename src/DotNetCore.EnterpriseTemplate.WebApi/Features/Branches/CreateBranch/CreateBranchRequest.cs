using System.ComponentModel.DataAnnotations;

namespace DotNetCore.EnterpriseTemplate.WebApi.Features.Branches.CreateBranch;

public class CreateBranchRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(200)]
    public string Address { get; set; }

    [Required]
    [StringLength(20)]
    public string Phone { get; set; }

    [Required]
    [StringLength(100)]
    public string Manager { get; set; }
}