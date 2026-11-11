using MediatR;
using NSubstitute;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Application.Products.Commands.RepriceProduct;
using PriceFlow.Application.Products.Events;
using PriceFlow.Domain.Products;

namespace PriceFlow.UnitTests.Application;

public class RepriceProductCommandHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly ICompetitorPriceClient _competitor = Substitute.For<ICompetitorPriceClient>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly RepriceProductCommandHandler _handler;

    private readonly Product _product = Product.Create("TEA-1001", "Green tea", 100m, 20m, DateTime.UtcNow);

    public RepriceProductCommandHandlerTests()
    {
        _handler = new RepriceProductCommandHandler(_products, _competitor, _unitOfWork, _publisher, TimeProvider.System);
        _products.GetByIdAsync(_product.Id, Arg.Any<CancellationToken>()).Returns(_product);
    }

    [Fact]
    public async Task Saves_new_price_and_publishes_event()
    {
        _competitor.GetPriceAsync("TEA-1001", Arg.Any<CancellationToken>()).Returns(200m);

        var result = await _handler.Handle(new RepriceProductCommand(_product.Id), CancellationToken.None);

        Assert.True(result.PriceChanged);
        Assert.Equal(198m, result.Price);
        _products.Received(1).AddPriceChange(Arg.Is<PriceChange>(c => c.NewPrice == 198m));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(
            Arg.Is<PriceChangedNotification>(n => n.Sku == "TEA-1001" && n.NewPrice == 198m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Does_not_publish_when_price_stays_the_same()
    {
        _competitor.GetPriceAsync("TEA-1001", Arg.Any<CancellationToken>()).Returns(110m);

        var result = await _handler.Handle(new RepriceProductCommand(_product.Id), CancellationToken.None);

        Assert.False(result.PriceChanged);
        _products.DidNotReceive().AddPriceChange(Arg.Any<PriceChange>());
        await _publisher.DidNotReceive().Publish(Arg.Any<PriceChangedNotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Keeps_price_when_competitor_does_not_sell_the_product()
    {
        _competitor.GetPriceAsync("TEA-1001", Arg.Any<CancellationToken>()).Returns((decimal?)null);

        var result = await _handler.Handle(new RepriceProductCommand(_product.Id), CancellationToken.None);

        Assert.False(result.PriceChanged);
        Assert.Equal(120m, result.Price);
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_not_found_for_unknown_product()
    {
        var command = new RepriceProductCommand(Guid.NewGuid());

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
