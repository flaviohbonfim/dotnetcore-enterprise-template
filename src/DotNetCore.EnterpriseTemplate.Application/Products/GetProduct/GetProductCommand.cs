using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Products.GetProduct;

public class GetProductCommand : IRequest<GetProductResult>
{
    public Guid Id { get; set; }
}