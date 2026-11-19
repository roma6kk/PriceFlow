using MediatR;
using NSubstitute;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Application.Products.Commands.ChangeProductCost;
using PriceFlow.Application.Products.Events;
using PriceFlow.Domain.Products;

namespace PriceFlow.UnitTests.Application;

public class ChangeProductCostCommandHandlerTests
{
    private readonly IProductRepository _products = Substitute.For<IProductRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IPublisher _publisher = Substitute.For<IPublisher>();
    private readonly ChangeProductCostCommandHandler _handler;

    private readonly Product _product = Product.Create("TEA-1001", "Green tea", 100m, 20m, DateTime.UtcNow);

    public ChangeProductCostCommandHandlerTests()
    {
        _handler = new ChangeProductCostCommandHandler(_products, _unitOfWork, _publisher, TimeProvider.System);
        _products.GetByIdAsync(_product.Id, Arg.Any<CancellationToken>()).Returns(_product);
    }

    [Fact]
    public async Task Raises_price_when_cost_goes_up()
    {
        await _handler.Handle(new ChangeProductCostCommand(_product.Id, 150m, 20m), CancellationToken.None);

        Assert.Equal(180m, _product.Price);
        _products.Received(1).AddPriceChange(Arg.Is<PriceChange>(c => c.NewPrice == 180m));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.Received(1).Publish(Arg.Any<PriceChangedNotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Saves_but_does_not_publish_when_price_stays_the_same()
    {
        await _handler.Handle(new ChangeProductCostCommand(_product.Id, 100m, 20m), CancellationToken.None);

        _products.DidNotReceive().AddPriceChange(Arg.Any<PriceChange>());
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        await _publisher.DidNotReceive().Publish(Arg.Any<PriceChangedNotification>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_not_found_for_unknown_product()
    {
        var command = new ChangeProductCostCommand(Guid.NewGuid(), 150m, 20m);

        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}
