using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using DotNetCore.EnterpriseTemplate.WebApi.Common;
using DotNetCore.EnterpriseTemplate.Application.Customers.GetCustomer;
using DotNetCore.EnterpriseTemplate.Application.Customers.GetCustomers;
using DotNetCore.EnterpriseTemplate.Application.Customers.CreateCustomer;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Customers.CreateCustomer;

namespace DotNetCore.EnterpriseTemplate.WebApi.Features.Customers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomersController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CustomersController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<List<GetCustomersResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCustomersCommand(), cancellationToken);
        return Ok(new ApiResponseWithData<List<GetCustomersResult>>
        {
            Success = true,
            Message = "Customers retrieved successfully",
            Data = result
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetCustomerResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCustomer([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCustomerCommand { Id = id }, cancellationToken);
        return Ok(new ApiResponseWithData<GetCustomerResult>
        {
            Success = true,
            Message = "Customer retrieved successfully",
            Data = result
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateCustomerResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateCustomerCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateCustomerResult>
        {
            Success = true,
            Message = "Customer created successfully",
            Data = response
        });
    }
}