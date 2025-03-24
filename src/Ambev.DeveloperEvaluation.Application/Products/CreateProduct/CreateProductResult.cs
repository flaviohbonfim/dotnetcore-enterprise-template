namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string SKU { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}