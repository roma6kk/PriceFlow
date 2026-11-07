using FluentValidation;
using PriceFlow.Application.Common.Behaviors;
using PriceFlow.Application.Products.Commands.CreateProduct;

namespace PriceFlow.UnitTests.Application;

public class ValidationBehaviorTests
{
    private readonly ValidationBehavior<CreateProductCommand, Guid> _behavior =
        new([new CreateProductCommandValidator()]);

    [Fact]
    public async Task Passes_valid_request_to_the_handler()
    {
        var command = new CreateProductCommand("TEA-1001", "Green tea", 100m, 20m);
        var expected = Guid.NewGuid();

        var result = await _behavior.Handle(command, () => Task.FromResult(expected), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Stops_invalid_request_before_the_handler()
    {
        var command = new CreateProductCommand("tea 1001!", "", -5m, 20m);
        var handlerCalled = false;

        var ex = await Assert.ThrowsAsync<ValidationException>(() => _behavior.Handle(command, () =>
        {
            handlerCalled = true;
            return Task.FromResult(Guid.NewGuid());
        }, CancellationToken.None));

        Assert.False(handlerCalled);
        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(CreateProductCommand.Sku));
        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(CreateProductCommand.Name));
        Assert.Contains(ex.Errors, e => e.PropertyName == nameof(CreateProductCommand.Cost));
    }
}
