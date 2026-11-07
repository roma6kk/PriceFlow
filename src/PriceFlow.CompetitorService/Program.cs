using PriceFlow.CompetitorService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<PriceSimulator>();

var app = builder.Build();

app.MapGrpcService<CompetitorPricesService>();
app.MapGet("/", () => "PriceFlow competitor price service. Talk to it over gRPC.");

app.Run();
