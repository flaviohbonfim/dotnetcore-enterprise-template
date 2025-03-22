using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a product in the system.
    /// </summary>
    public class Product : BaseEntity
    {
        /// <summary>
        /// Gets or sets the product's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product's description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the product's price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the product's SKU (Stock Keeping Unit).
        /// </summary>
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the Product class.
        /// </summary>
        public Product() { }
    }
}
