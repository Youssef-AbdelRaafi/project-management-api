using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.Common.Behaviors;

/// <summary>
/// Logs application requests that exceed the expected execution threshold.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUserService currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int ThresholdMilliseconds = 500;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();
        var response = await next();
        timer.Stop();

        if (timer.ElapsedMilliseconds > ThresholdMilliseconds)
        {
            var requestName = typeof(TRequest).Name;
            var sanitizedRequest = RequestLogSanitizer.Sanitize(request);

            logger.LogWarning(
                "Long running request: {RequestName} ({ElapsedMilliseconds} ms) by user {UserId} {@Request}",
                requestName,
                timer.ElapsedMilliseconds,
                currentUser.UserId,
                sanitizedRequest);
        }

        return response;
    }
}
