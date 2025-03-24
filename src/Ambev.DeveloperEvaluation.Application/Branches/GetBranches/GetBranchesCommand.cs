using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Branches.GetBranches;

public class GetBranchesCommand : IRequest<List<GetBranchesResult>>
{
}