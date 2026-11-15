using PriceFlow.Api.Infrastructure;
using PriceFlow.Api.Workers;
using PriceFlow.Application;
using PriceFlow.Infrastructure;
using PriceFlow.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

builder.Services.Configure<RepricingOptions>(builder.Configuration.GetSection(RepricingOptions.Section));
builder.Services.AddHostedService<RepricingWorker>();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync(seedDemoData: app.Environment.IsDevelopment());

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
