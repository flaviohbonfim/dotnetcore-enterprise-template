namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public class CreateSaleRequest
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? CustomerId { get; set; }
    public List<CreateSaleItemRequest> Items { get; set; } = new();
}

public class CreateSaleItemRequest
{
    public Guid? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}