using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Branches.GetBranches;

public class GetBranchesHandler : IRequestHandler<GetBranchesCommand, List<GetBranchesResult>>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetBranchesHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<List<GetBranchesResult>> Handle(GetBranchesCommand request, CancellationToken cancellationToken)
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<GetBranchesResult>>(branches);
    }
}