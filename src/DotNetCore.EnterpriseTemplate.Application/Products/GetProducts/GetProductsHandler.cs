using AutoMapper;
using MediatR;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;

namespace DotNetCore.EnterpriseTemplate.Application.Products.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsCommand, List<GetProductsResult>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<List<GetProductsResult>> Handle(GetProductsCommand request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<List<GetProductsResult>>(products);
    }
}