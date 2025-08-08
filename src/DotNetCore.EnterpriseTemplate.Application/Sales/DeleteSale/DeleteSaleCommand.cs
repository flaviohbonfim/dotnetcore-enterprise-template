using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Sales.DeleteSale;

public class DeleteSaleCommand : IRequest<DeleteSaleResult>
{
    public Guid Id { get; set; }
    public bool IsActive { get; set; }
}