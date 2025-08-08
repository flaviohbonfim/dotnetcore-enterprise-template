using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Branches.GetBranch;

public class GetBranchCommand : IRequest<GetBranchResult>
{
    public Guid Id { get; set; }
}