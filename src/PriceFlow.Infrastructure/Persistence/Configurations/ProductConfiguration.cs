using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceFlow.Domain.Products;

namespace PriceFlow.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Sku).HasMaxLength(64);
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.Property(p => p.Name).HasMaxLength(200);

        builder.Property(p => p.Cost).HasPrecision(18, 2);
        builder.Property(p => p.MinMarginPercent).HasPrecision(5, 2);
        builder.Property(p => p.Price).HasPrecision(18, 2);
        builder.Property(p => p.CompetitorPrice).HasPrecision(18, 2);

        // Maps to the xmin system column: PostgreSQL bumps it on every row update,
        // so EF Core gets optimistic concurrency without an extra column.
        builder.Property(p => p.Version).IsRowVersion();
    }
}
