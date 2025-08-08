using DotNetCore.EnterpriseTemplate.Domain.Common;

namespace DotNetCore.EnterpriseTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a customer in the system.
    /// </summary>
    public class Customer : BaseEntity
    {
        /// <summary>
        /// Gets or sets the customer's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's document.
        /// </summary>
        public string Document { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's phone number.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the customer's address.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the Customer class.
        /// </summary>
        public Customer() { }
    }
}
