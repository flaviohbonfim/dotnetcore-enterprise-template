using AutoMapper;
using DotNetCore.EnterpriseTemplate.Domain.Entities;
using DotNetCore.EnterpriseTemplate.Application.Sales.CreateSale;
using DotNetCore.EnterpriseTemplate.WebApi.Features.Sales.CreateSale;
using DotNetCore.EnterpriseTemplate.Application.Sales.CancelSale;
using DotNetCore.EnterpriseTemplate.Application.Sales.DeleteSale;

namespace DotNetCore.EnterpriseTemplate.WebApi.Mappings;

public class CreateSaleProfile : Profile
{
    public CreateSaleProfile()
    {
        // API Request to Command
        CreateMap<CreateSaleRequest, CreateSaleCommand>();
        CreateMap<CreateSaleItemRequest, CreateSaleItemCommand>();

        // Command to Domain
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId))
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Branch, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<CreateSaleItemCommand, SaleItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.SaleId, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Product, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => 
                (src.Quantity * src.UnitPrice) - src.Discount));

        // Domain to Result
        CreateMap<Sale, CreateSaleResult>();
        CreateMap<SaleItem, CreateSaleItemResult>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Name : string.Empty));

        CreateMap<Sale, CancelSaleResult>();

        CreateMap<Sale, DeleteSaleResult>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}
