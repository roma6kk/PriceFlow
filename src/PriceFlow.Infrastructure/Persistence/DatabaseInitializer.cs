using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PriceFlow.Domain.Products;

namespace PriceFlow.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider services, bool seedDemoData)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.Database.MigrateAsync();

        if (!seedDemoData || await db.Products.AnyAsync())
            return;

        var now = DateTime.UtcNow;
        db.Products.AddRange(
            Product.Create("TEA-1027", "Green tea, 100 bags", 340m, 15m, now),
            Product.Create("COF-1126", "Ground coffee, 250 g", 480m, 20m, now),
            Product.Create("CHO-1194", "Dark chocolate 72%, 90 g", 150m, 25m, now),
            Product.Create("WAT-1047", "Mineral water, 1.5 l x 6", 380m, 10m, now),
            Product.Create("EXCL-5005", "Own brand granola, 400 g", 240m, 30m, now));

        await db.SaveChangesAsync();
    }
}
