using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PriceFlow.Application.Common.Interfaces;
using PriceFlow.Contracts.Grpc;
using PriceFlow.Infrastructure.Competitors;
using PriceFlow.Infrastructure.Persistence;

namespace PriceFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres"))
                .UseSnakeCaseNamingConvention());
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddGrpcClient<CompetitorPrices.CompetitorPricesClient>(options =>
            options.Address = new Uri(configuration["CompetitorService:Url"]!));
        services.AddScoped<ICompetitorPriceClient, CompetitorPriceGrpcClient>();

        return services;
    }
}
