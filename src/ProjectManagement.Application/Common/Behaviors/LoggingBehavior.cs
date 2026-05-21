using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using ProjectManagement.Application.Common.Interfaces;

namespace ProjectManagement.Application.Common.Behaviors;

/// <summary>
/// Logs the start and successful completion of MediatR request execution.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger,
    ICurrentUserService currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation(
            "Handling application request {RequestName} by user {UserId}",
            requestName,
            currentUser.UserId);

        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        logger.LogInformation(
            "Application request {RequestName} completed successfully in {ElapsedMilliseconds} ms by user {UserId}",
            requestName,
            stopwatch.ElapsedMilliseconds,
            currentUser.UserId);

        return response;
    }
}
