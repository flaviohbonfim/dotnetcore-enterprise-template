using AutoMapper;
using MediatR;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Domain.Repositories;

namespace DotNetCore.EnterpriseTemplate.Application.Products.CreateProduct;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            SKU = request.SKU,
            IsActive = true
        };

        product = await _productRepository.CreateAsync(product, cancellationToken);
        return _mapper.Map<CreateProductResult>(product);
    }
}