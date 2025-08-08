using DotNetCore.EnterpriseTemplate.Domain.Common;

namespace DotNetCore.EnterpriseTemplate.Domain.Entities
{
    /// <summary>
    /// Represents a branch (store or location) in the system.
    /// </summary>
    public class Branch : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name of the branch.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the address of the branch.
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the branch's phone number.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manager's name of the branch.
        /// </summary>
        public string Manager { get; set; } = string.Empty;

        /// <summary>
        /// Initializes a new instance of the Branch class.
        /// </summary>
        public Branch() { }
    }
}
