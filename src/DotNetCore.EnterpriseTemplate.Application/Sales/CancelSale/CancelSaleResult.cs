namespace DotNetCore.EnterpriseTemplate.Application.Sales.CancelSale;

public class CancelSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; }
    public DateTime CancelledAt { get; set; }
}