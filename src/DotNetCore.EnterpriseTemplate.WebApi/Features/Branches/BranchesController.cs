using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using DotNetCore.EnterpriseTemplate.WebApi.Common;
using DotNetCore.EnterpriseTemplate.Application.Branches.GetBranch;
using DotNetCore.EnterpriseTemplate.Application.Branches.GetBranches;
using DotNetCore.EnterpriseTemplate.Application.Branches.CreateBranch;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Branches.CreateBranch;

namespace DotNetCore.EnterpriseTemplate.WebApi.Features.Branches;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BranchesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public BranchesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<List<GetBranchesResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBranches(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBranchesCommand(), cancellationToken);
        return Ok(new ApiResponseWithData<List<GetBranchesResult>>
        {
            Success = true,
            Message = "Branches retrieved successfully",
            Data = result
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetBranchResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetBranchCommand { Id = id }, cancellationToken);
        return Ok(new ApiResponseWithData<GetBranchResult>
        {
            Success = true,
            Message = "Branch retrieved successfully",
            Data = result
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateBranchResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBranch([FromBody] CreateBranchRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateBranchCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateBranchResult>
        {
            Success = true,
            Message = "Branch created successfully",
            Data = response
        });
    }
}