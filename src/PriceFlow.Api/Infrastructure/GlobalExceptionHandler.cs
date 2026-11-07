using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PriceFlow.Application.Common.Exceptions;
using PriceFlow.Domain.Common;

namespace PriceFlow.Api.Infrastructure;

/// <summary>Turns application exceptions into RFC 7807 problem details.</summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var problem = exception switch
        {
            ValidationException e => new ValidationProblemDetails(e.Errors
                .GroupBy(f => f.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray()))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed"
            },
            NotFoundException e => Problem(StatusCodes.Status404NotFound, "Not found", e.Message),
            ConflictException e => Problem(StatusCodes.Status409Conflict, "Conflict", e.Message),
            DomainException e => Problem(StatusCodes.Status422UnprocessableEntity, "Business rule violated", e.Message),
            _ => null
        };

        if (problem is null)
        {
            logger.LogError(exception, "Unhandled exception");
            problem = Problem(StatusCodes.Status500InternalServerError, "Internal server error", null);
        }

        context.Response.StatusCode = problem.Status!.Value;
        await context.Response.WriteAsJsonAsync<object>(problem, ct);
        return true;
    }

    private static ProblemDetails Problem(int status, string title, string? detail) =>
        new() { Status = status, Title = title, Detail = detail };
}
