using MediatR;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Domain.Products;

namespace PriceFlow.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(string Sku, string Name, decimal Cost, decimal MinMarginPercent)
    : IRequest<Guid>;

public class CreateProductCommandHandler(IProductRepository products, IUnitOfWork unitOfWork, TimeProvider time)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken ct)
    {
        var product = Product.Create(command.Sku, command.Name, command.Cost,
            command.MinMarginPercent, time.GetUtcNow().UtcDateTime);

        if (await products.GetBySkuAsync(product.Sku, ct) is not null)
            throw new ConflictException($"Product with SKU '{product.Sku}' already exists.");

        products.Add(product);
        await unitOfWork.SaveChangesAsync(ct);

        return product.Id;
    }
}
