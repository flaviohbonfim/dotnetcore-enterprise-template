namespace Ambev.DeveloperEvaluation.Application.Branches.GetBranch;

public class GetBranchResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool IsActive { get; set; }
}