using PriceFlow.Domain.Products;

namespace PriceFlow.Application.Common.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<PriceChange>> GetPriceHistoryAsync(Guid productId, int take, CancellationToken ct);
    void Add(Product product);
    void AddPriceChange(PriceChange change);
}
