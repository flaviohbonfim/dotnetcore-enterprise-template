using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using DotNetCore.EnterpriseTemplate.WebApi.Common;
using DotNetCore.EnterpriseTemplate.Application.Products.GetProduct;
using DotNetCore.EnterpriseTemplate.Application.Products.GetProducts;
using DotNetCore.EnterpriseTemplate.Application.Products.CreateProduct;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Products.CreateProduct;

namespace DotNetCore.EnterpriseTemplate.WebApi.Features.Products;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProductsController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<List<GetProductsResult>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductsCommand(), cancellationToken);
        return Ok(new ApiResponseWithData<List<GetProductsResult>>
        {
            Success = true,
            Message = "Products retrieved successfully",
            Data = result
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetProductResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductCommand { Id = id }, cancellationToken);
        return Ok(new ApiResponseWithData<GetProductResult>
        {
            Success = true,
            Message = "Product retrieved successfully",
            Data = result
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateProductResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateProductCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateProductResult>
        {
            Success = true,
            Message = "Product created successfully",
            Data = response
        });
    }
}