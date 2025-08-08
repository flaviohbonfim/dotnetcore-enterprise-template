using AutoMapper;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Application.Products.GetProduct;
using DotNetCore.EnterpriseTemplate.Application.Products.GetProducts;
using DotNetCore.EnterpriseTemplate.Application.Products.CreateProduct;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Products.CreateProduct;

namespace DotNetCore.EnterpriseTemplate.WebApi.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, GetProductResult>();
        CreateMap<Product, GetProductsResult>();
        CreateMap<CreateProductRequest, CreateProductCommand>();
        CreateMap<Product, CreateProductResult>();
    }
}