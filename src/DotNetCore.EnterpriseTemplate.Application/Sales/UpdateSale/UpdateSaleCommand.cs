using DotNetCore.EnterpriseTemplate.Application.Sales.CreateSale;
using MediatR;

namespace DotNetCore.EnterpriseTemplate.Application.Sales.UpdateSale;

public class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    public Guid SaleId { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public List<UpdateSaleItemCommand> Items { get; set; } = new();
}

public class UpdateSaleItemCommand
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}
