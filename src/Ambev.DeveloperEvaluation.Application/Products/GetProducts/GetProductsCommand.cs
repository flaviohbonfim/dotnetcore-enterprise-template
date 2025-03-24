using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProducts;

public class GetProductsCommand : IRequest<List<GetProductsResult>>
{
}