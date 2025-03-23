using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.TotalAmount)
                .IsRequired();

            builder.Property(s => s.SaleDate)
                .IsRequired();

            builder.HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId);

            builder.HasOne(s => s.Branch)
                .WithMany()
                .HasForeignKey(s => s.BranchId);

            builder.HasMany(s => s.Items)  // Relacionamento com SaleItem
                .WithOne(si => si.Sale)    // Cada SaleItem pertence a uma Sale
                .HasForeignKey(si => si.SaleId);

            builder.HasMany(s => s.Items)  // Relacionamento de Sale com SaleItems
                .WithOne(si => si.Sale)    // Cada SaleItem pertence a uma Sale
                .HasForeignKey(si => si.SaleId);

            // Agora, ao acessar os produtos, você pode usar Sale.Items, e cada SaleItem tem um Product
            builder.HasMany(s => s.Items)
                .WithOne()
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);  // Opção de exclusão em cascata, se necessário
        }
    }
}
