using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateSaleResult>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response
        });
    }

    /// <summary>
    /// Retrieves a sale by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new GetSaleCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetSaleResult>
        {
            Success = true,
            Message = "Sale retrieved successfully",
            Data = response
        });
    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSaleCommand>(request);
        command.SaleId = id;
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateSaleResult>
        {
            Success = true,
            Message = "Sale updated successfully",
            Data = response
        });
    }

    /// <summary>
    /// Deletes a sale
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSaleCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<DeleteSaleResult>
        {
            Success = true,
            Message = "Sale deleted successfully",
            Data = response
        });
    }

    /// <summary>
    /// Cancels a sale
    /// </summary>
    [HttpPut("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new CancelSaleCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CancelSaleResult>
        {
            Success = true,
            Message = "Sale cancelled successfully",
            Data = response
        });
    }

    /// <summary>
    /// Cancels a sale item
    /// </summary>
    [HttpPut("{saleId}/items/{itemId}/cancel")]
    [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleItemResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelSaleItem([FromRoute] Guid saleId, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    {
        var command = new CancelSaleItemCommand 
        { 
            SaleId = saleId,
            ItemId = itemId
        };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<CancelSaleItemResult>
        {
            Success = true,
            Message = "Sale item cancelled successfully",
            Data = response
        });
    }
}
