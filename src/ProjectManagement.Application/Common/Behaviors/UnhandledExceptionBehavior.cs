using MediatR;
using Microsoft.Extensions.Logging;

namespace ProjectManagement.Application.Common.Behaviors;

public sealed class UnhandledExceptionBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
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
