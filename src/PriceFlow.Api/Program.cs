using PriceFlow.Application;
using PriceFlow.Infrastructure;
using PriceFlow.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
