using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PriceFlow.Domain.Products;

namespace PriceFlow.Infrastructure.Persistence.Configurations;

public class PriceChangeConfiguration : IEntityTypeConfiguration<PriceChange>
{
    public void Configure(EntityTypeBuilder<PriceChange> builder)
    {
        builder.ToTable("price_changes");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.OldPrice).HasPrecision(18, 2);
        builder.Property(c => c.NewPrice).HasPrecision(18, 2);
        builder.Property(c => c.CompetitorPrice).HasPrecision(18, 2);
        builder.Property(c => c.Reason).HasConversion<string>().HasMaxLength(32);

        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
