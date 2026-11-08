using MediatR;
using Microsoft.AspNetCore.Mvc;
using PriceFlow.Application.Products;
using PriceFlow.Application.Products.Commands.CreateProduct;
using PriceFlow.Application.Products.Commands.RepriceProduct;
using PriceFlow.Application.Products.Queries.GetProductById;
using PriceFlow.Application.Products.Queries.GetProducts;

namespace PriceFlow.Api.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<IReadOnlyList<ProductDto>> GetAll(CancellationToken ct) =>
        sender.Send(new GetProductsQuery(), ct);

    [HttpGet("{id:guid}")]
    public Task<ProductDto> GetById(Guid id, CancellationToken ct) =>
        sender.Send(new GetProductByIdQuery(id), ct);

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Asks the competitor service for a fresh price and reprices the product right now.</summary>
    [HttpPost("{id:guid}/reprice")]
    public Task<RepriceResult> Reprice(Guid id, CancellationToken ct) =>
        sender.Send(new RepriceProductCommand(id), ct);
}
