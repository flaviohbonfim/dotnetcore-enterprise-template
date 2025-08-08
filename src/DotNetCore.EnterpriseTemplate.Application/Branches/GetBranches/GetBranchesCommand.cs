using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Branches.GetBranches;

public class GetBranchesCommand : IRequest<List<GetBranchesResult>>
{
}