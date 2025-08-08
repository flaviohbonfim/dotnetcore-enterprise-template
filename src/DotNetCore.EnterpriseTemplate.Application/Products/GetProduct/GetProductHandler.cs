using AutoMapper;
using MediatR;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;

namespace DotNetCore.EnterpriseTemplate.Application.Products.GetProduct;

public class GetProductHandler : IRequestHandler<GetProductCommand, GetProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<GetProductResult> Handle(GetProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (product == null)
            throw new InvalidOperationException($"Product with ID {request.Id} not found");

        return _mapper.Map<GetProductResult>(product);
    }
}