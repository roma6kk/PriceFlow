namespace PriceFlow.Application.Common.Exceptions;

public class NotFoundException(string entity, object key)
    : Exception($"{entity} '{key}' was not found.");
