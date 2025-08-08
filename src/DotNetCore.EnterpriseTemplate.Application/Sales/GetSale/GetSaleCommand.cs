using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Sales.GetSale;

public class GetSaleCommand : IRequest<GetSaleResult>
{
    public Guid Id { get; set; }
}