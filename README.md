# PriceFlow

[![CI](https://github.com/roma6kk/PriceFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/roma6kk/PriceFlow/actions/workflows/ci.yml)

Pet project for practicing .NET backend. A dynamic pricing service: keeps product prices a bit below the competitor's price, but never below cost plus minimal margin, and shows price changes on a live page.

## Services

- **PriceFlow.Api** - REST API, reprices the catalog every 15 seconds
- **PriceFlow.CompetitorService** - gRPC service with fake competitor prices
- **PriceFlow.Notifier** - reads price changes from Kafka and sends them to the browser over SignalR

Api -> CompetitorService over gRPC, Api -> Kafka -> Notifier -> browser.

## Stack

.NET 10, ASP.NET Core, MediatR, FluentValidation, EF Core + PostgreSQL, Redis, Kafka, gRPC, SignalR, xUnit, NSubstitute, Docker, GitHub Actions.

## Run

```bash
docker compose up --build
```

- Swagger: http://localhost:5080/swagger
- Live page: http://localhost:5082

Demo products are added on first start.

## Tests

```bash
dotnet test
```
