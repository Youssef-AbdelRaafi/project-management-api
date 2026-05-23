using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.Common.Behaviors;

public sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
    ICurrentUserService currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int ThresholdMilliseconds = 500;

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
