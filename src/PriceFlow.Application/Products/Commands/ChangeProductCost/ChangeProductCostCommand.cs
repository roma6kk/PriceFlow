using FluentValidation;
using MediatR;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Application.Products.Events;
using PriceFlow.Domain.Products;

namespace PriceFlow.Application.Products.Commands.ChangeProductCost;

public record ChangeProductCostCommand(Guid ProductId, decimal Cost, decimal MinMarginPercent) : IRequest;

public class ChangeProductCostCommandValidator : AbstractValidator<ChangeProductCostCommand>
{
    public ChangeProductCostCommandValidator()
    {
        RuleFor(x => x.Cost).GreaterThan(0);
        RuleFor(x => x.MinMarginPercent).InclusiveBetween(0, 500);
    }
}

public class ChangeProductCostCommandHandler(
    IProductRepository products,
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    TimeProvider time) : IRequestHandler<ChangeProductCostCommand>
{
    public async Task Handle(ChangeProductCostCommand command, CancellationToken ct)
    {
        var product = await products.GetByIdAsync(command.ProductId, ct)
            ?? throw new NotFoundException(nameof(Product), command.ProductId);

        var change = product.ChangeCost(command.Cost, command.MinMarginPercent, time.GetUtcNow().UtcDateTime);
        if (change is not null)
            products.AddPriceChange(change);

        await unitOfWork.SaveChangesAsync(ct);

        if (change is not null)
            await publisher.Publish(PriceChangedNotification.From(product, change), ct);
    }
}
