using FluentValidation;
using MediatR;
using PriceFlow.Application.Common.Interfaces;

namespace PriceFlow.Application.Products.Queries.GetPriceHistory;

public record GetPriceHistoryQuery(Guid ProductId, int Take = 50) : IRequest<IReadOnlyList<PriceChangeDto>>;

public record PriceChangeDto(decimal OldPrice, decimal NewPrice, decimal? CompetitorPrice, string Reason, DateTime ChangedAt);

public class GetPriceHistoryQueryValidator : AbstractValidator<GetPriceHistoryQuery>
{
    public GetPriceHistoryQueryValidator()
    {
        RuleFor(x => x.Take).InclusiveBetween(1, 500);
    }
}

public class GetPriceHistoryQueryHandler(IProductRepository products)
    : IRequestHandler<GetPriceHistoryQuery, IReadOnlyList<PriceChangeDto>>
{
    public async Task<IReadOnlyList<PriceChangeDto>> Handle(GetPriceHistoryQuery query, CancellationToken ct)
    {
        var history = await products.GetPriceHistoryAsync(query.ProductId, query.Take, ct);

        return history
            .Select(c => new PriceChangeDto(c.OldPrice, c.NewPrice, c.CompetitorPrice, c.Reason.ToString(), c.ChangedAt))
            .ToList();
    }
}
