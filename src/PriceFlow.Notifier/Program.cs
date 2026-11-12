using PriceFlow.Notifier.Consumers;
using PriceFlow.Notifier.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddHostedService<PriceChangedConsumer>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapHub<PricesHub>("/hubs/prices");

app.Run();
