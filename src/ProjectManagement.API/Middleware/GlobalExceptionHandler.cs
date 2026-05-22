using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using ProjectManagement.Application.Common.Models;
using ProjectManagement.Domain.Exceptions;
using ResultStatusCodes = ProjectManagement.Application.Common.Models.StatusCodes;

namespace ProjectManagement.API.Middleware;

/// <summary>
/// Converts unhandled exceptions into the API's standard result response shape.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private const string UnexpectedErrorMessage = "An unexpected error occurred.";

    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var result = CreateResult(exception);

        LogException(exception, result.StatusCode);

        httpContext.Response.StatusCode = result.StatusCode;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(result, cancellationToken);

        return true;
    }

    private static Result<object> CreateResult(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => Result<object>.ValidationFailure(
                validationException.Errors
                    .Select(error => error.ErrorMessage)
                    .Distinct()
                    .ToList()),

            NotFoundException notFoundException => Result<object>.Failure(
                notFoundException.Message,
                ResultStatusCodes.NotFound),

            ForbiddenException forbiddenException => Result<object>.Failure(
                forbiddenException.Message,
                ResultStatusCodes.Forbidden),

            UnauthorizedException unauthorizedException => Result<object>.Failure(
                unauthorizedException.Message,
                ResultStatusCodes.Unauthorized),

            DomainException domainException => Result<object>.Failure(
                domainException.Message,
                ResultStatusCodes.UnprocessableEntity),

            _ => Result<object>.Failure(
                UnexpectedErrorMessage,
                ResultStatusCodes.InternalServerError)
        };
    }

    private void LogException(Exception exception, int statusCode)
    {
        if (statusCode >= ResultStatusCodes.InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception occurred");
            return;
        }

        logger.LogWarning(
            exception,
            "Handled exception mapped to status code {StatusCode}",
            statusCode);
    }
}
