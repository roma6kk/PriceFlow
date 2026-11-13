using MediatR;
using Microsoft.AspNetCore.Mvc;
using PriceFlow.Application.Products.Queries.GetCurrentPrice;

namespace PriceFlow.Api.Controllers;

[ApiController]
[Route("api/prices")]
public class PricesController(ISender sender) : ControllerBase
{
    /// <summary>Current price by SKU. Served from Redis when possible.</summary>
    [HttpGet("{sku}")]
    public Task<CurrentPriceDto> Get(string sku, CancellationToken ct) =>
        sender.Send(new GetCurrentPriceQuery(sku), ct);
}
