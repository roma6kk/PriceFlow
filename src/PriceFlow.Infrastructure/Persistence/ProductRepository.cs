using Microsoft.EntityFrameworkCore;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Domain.Products;

namespace PriceFlow.Infrastructure.Persistence;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken ct) =>
        db.Products.FirstOrDefaultAsync(p => p.Sku == sku, ct);

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct) =>
        await db.Products.OrderBy(p => p.Sku).ToListAsync(ct);

    public async Task<IReadOnlyList<PriceChange>> GetPriceHistoryAsync(Guid productId, int take, CancellationToken ct) =>
        await db.PriceChanges
            .AsNoTracking()
            .Where(c => c.ProductId == productId)
            .OrderByDescending(c => c.ChangedAt)
            .Take(take)
            .ToListAsync(ct);

    public void Add(Product product) => db.Products.Add(product);

    public void AddPriceChange(PriceChange change) => db.PriceChanges.Add(change);
}
