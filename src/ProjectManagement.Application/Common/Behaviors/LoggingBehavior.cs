using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ProjectManagement.Application.Common.Behaviors;

/// <summary>
/// Logs MediatR request execution without serializing potentially sensitive payloads.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const long SlowRequestThresholdMilliseconds = 500;

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Handling application request {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > SlowRequestThresholdMilliseconds)
            {
                logger.LogWarning(
                    "Application request {RequestName} completed successfully in {ElapsedMilliseconds} ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                logger.LogInformation(
                    "Application request {RequestName} completed successfully in {ElapsedMilliseconds} ms",
                    requestName,
                    stopwatch.ElapsedMilliseconds);
            }

            return response;
        }
        catch (Exception exception)
        {
            stopwatch.Stop();

            logger.LogError(
                exception,
                "Application request {RequestName} failed after {ElapsedMilliseconds} ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
