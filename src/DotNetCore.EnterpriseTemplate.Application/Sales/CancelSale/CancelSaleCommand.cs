using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Sales.CancelSale;

public class CancelSaleCommand : IRequest<CancelSaleResult>
{
    public Guid Id { get; set; }
}