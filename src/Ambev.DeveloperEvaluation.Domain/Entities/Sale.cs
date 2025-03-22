using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    /// <summary>
    /// Represents a sale in the system with its related details.
    /// This entity follows domain-driven design principles and includes business rules validation.
    /// </summary>
    public class Sale : BaseEntity
    {
        /// <summary>
        /// Gets the sale's number.
        /// This should be a unique identifier for each sale.
        /// </summary>
        public string SaleNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets the date when the sale was made.
        /// </summary>
        public DateTime SaleDate { get; set; }

        /// <summary>
        /// Gets the customer related to the sale.
        /// </summary>
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = new();

        /// <summary>
        /// Gets the branch where the sale occurred.
        /// </summary>
        public Guid BranchId { get; set; }
        public Branch Branch { get; set; } = new();

        /// <summary>
        /// Gets the list of items in the sale.
        /// </summary>
        public List<SaleItem> Items { get; set; } = new();

        /// <summary>
        /// Gets the total sale amount.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Indicates whether the sale has been cancelled or not.
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Gets the date and time when the sale was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets the date and time of the last update to the sale information.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the Sale class.
        /// </summary>
        public Sale()
        {
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Performs validation of the sale entity using SaleValidator rules.
        /// </summary>
        /// <returns>
        /// A ValidationResultDetail containing the validation result.
        /// </returns>
        public ValidationResultDetail Validate()
        {
            var validator = new SaleValidator();
            var result = validator.Validate(this);
            return new ValidationResultDetail
            {
                IsValid = result.IsValid,
                Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
            };
        }

        /// <summary>
        /// Applies the discount and calculates the total amount for the sale.
        /// </summary>
        public void ApplyDiscounts()
        {
            foreach (var item in Items)
            {
                if (item.Quantity >= 4 && item.Quantity <= 20)
                {
                    if (item.Quantity >= 10)
                    {
                        item.Discount = 0.20m; // 20% discount for quantities between 10 and 20
                    }
                    else
                    {
                        item.Discount = 0.10m; // 10% discount for quantities between 4 and 9
                    }

                    item.TotalPrice = item.Quantity * item.UnitPrice * (1 - item.Discount);
                }
                else
                {
                    item.Discount = 0m; // No discount for quantities less than 4
                    item.TotalPrice = item.Quantity * item.UnitPrice;
                }
            }

            TotalAmount = Items.Sum(i => i.TotalPrice);
        }

        /// <summary>
        /// Cancels the sale by marking it as cancelled and updating the date.
        /// </summary>
        public void CancelSale()
        {
            IsCancelled = true;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Updates the sale status to 'Completed'.
        /// </summary>
        public void CompleteSale()
        {
            IsCancelled = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
