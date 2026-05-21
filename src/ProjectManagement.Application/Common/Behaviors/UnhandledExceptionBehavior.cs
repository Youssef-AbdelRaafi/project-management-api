using MediatR;
using Microsoft.Extensions.Logging;

namespace ProjectManagement.Application.Common.Behaviors;

/// <summary>
/// Logs unexpected MediatR exceptions before they reach the API global exception handler.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class UnhandledExceptionBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception exception)
        {
            var requestName = typeof(TRequest).Name;
            var sanitizedRequest = RequestLogSanitizer.Sanitize(request);

            logger.LogError(
                exception,
                "Unhandled exception for request {RequestName} {@Request}",
                requestName,
                sanitizedRequest);

            throw;
        }
    }
}
