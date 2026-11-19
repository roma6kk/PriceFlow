using NSubstitute;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Application.Products.Queries.GetCurrentPrice;
using PriceFlow.Domain.Products;

namespace PriceFlow.UnitTests.Application;

public class GetCurrentPriceQueryHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly IPriceCache _cache = Substitute.For<IPriceCache>();
    private readonly GetCurrentPriceQueryHandler _handler;

    public GetCurrentPriceQueryHandlerTests()
    {
        _handler = new GetCurrentPriceQueryHandler(_products, _cache);
    }

    [Fact]
    public async Task Returns_cached_price_without_going_to_database()
    {
        _cache.GetAsync("TEA-1001", Arg.Any<CancellationToken>()).Returns(150m);

        var result = await _handler.Handle(new GetCurrentPriceQuery("TEA-1001"), CancellationToken.None);

        Assert.Equal(150m, result.Price);
        await _products.DidNotReceive().GetBySkuAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Reads_database_and_fills_cache_on_miss()
    {
        var product = Product.Create("TEA-1001", "Green tea", 100m, 20m, DateTime.UtcNow);
        _cache.GetAsync("TEA-1001", Arg.Any<CancellationToken>()).Returns((decimal?)null);
        _products.GetBySkuAsync("TEA-1001", Arg.Any<CancellationToken>()).Returns(product);

        var result = await _handler.Handle(new GetCurrentPriceQuery(" tea-1001 "), CancellationToken.None);

        Assert.Equal("TEA-1001", result.Sku);
        Assert.Equal(120m, result.Price);
        await _cache.Received(1).SetAsync("TEA-1001", 120m, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_not_found_for_unknown_sku()
    {
        _cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((decimal?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _handler.Handle(new GetCurrentPriceQuery("NOPE-1"), CancellationToken.None));
        await _cache.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<CancellationToken>());
    }
}
