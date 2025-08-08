using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Products.GetProducts;

public class GetProductsCommand : IRequest<List<GetProductsResult>>
{
}