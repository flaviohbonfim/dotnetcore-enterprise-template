using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; set; }
        public Sale? Sale { get; set; }  // Tornando opcional a referência à Sale.

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }  // Tornando opcional a referência ao Product.
        
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalPrice { get; set; }

        public SaleItem() { }
    }
}
