namespace DotNetCore.EnterpriseTemplate.Application.Branches.CreateBranch;

public class CreateBranchResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Manager { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}