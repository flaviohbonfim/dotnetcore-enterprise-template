using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Branches.CreateBranch;

public class CreateBranchCommand : IRequest<CreateBranchResult>
{
    public string Name { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string Manager { get; set; }
}