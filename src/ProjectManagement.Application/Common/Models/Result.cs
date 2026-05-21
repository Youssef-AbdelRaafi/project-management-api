namespace ProjectManagement.Application.Common.Models;

/// <summary>
/// Represents the outcome of an operation that does not return data.
/// </summary>
public class Result
{
    private const string ValidationErrorMessage = "Validation failed.";

    private Result(bool isSuccess, string? error, List<string> errors, int statusCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the primary error message when the operation fails.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets all validation or business errors returned by the operation.
    /// </summary>
    public List<string> Errors { get; }

    /// <summary>
    /// Gets the HTTP status code that best represents the operation outcome.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="statusCode">The success status code.</param>
    /// <returns>A successful result.</returns>
    public static Result Success(int statusCode = StatusCodes.Ok)
    {
        return new Result(true, null, [], statusCode);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The failure message.</param>
    /// <param name="statusCode">The failure status code.</param>
    /// <returns>A failed result.</returns>
    public static Result Failure(string error, int statusCode = StatusCodes.BadRequest)
    {
        return new Result(false, error, [error], statusCode);
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A failed validation result.</returns>
    public static Result ValidationFailure(List<string> errors)
    {
        return new Result(false, ValidationErrorMessage, errors, StatusCodes.BadRequest);
    }
}

/// <summary>
/// Represents the outcome of an operation that returns data.
/// </summary>
/// <typeparam name="T">The returned data type.</typeparam>
public sealed class Result<T>
{
    private const string ValidationErrorMessage = "Validation failed.";

    private Result(T? data, bool isSuccess, string? error, List<string> errors, int statusCode)
    {
        Data = data;
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets a value indicating whether the operation completed successfully.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets the returned data when the operation succeeds.
    /// </summary>
    public T? Data { get; }

    /// <summary>
    /// Gets the primary error message when the operation fails.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets all validation or business errors returned by the operation.
    /// </summary>
    public List<string> Errors { get; }

    /// <summary>
    /// Gets the HTTP status code that best represents the operation outcome.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    /// <param name="data">The operation data.</param>
    /// <param name="statusCode">The success status code.</param>
    /// <returns>A successful result.</returns>
    public static Result<T> Success(T data, int statusCode = StatusCodes.Ok)
    {
        return new Result<T>(data, true, null, [], statusCode);
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The failure message.</param>
    /// <param name="statusCode">The failure status code.</param>
    /// <returns>A failed result.</returns>
    public static Result<T> Failure(string error, int statusCode = StatusCodes.BadRequest)
    {
        return new Result<T>(default, false, error, [error], statusCode);
    }

    /// <summary>
    /// Creates a failed validation result.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    /// <returns>A failed validation result.</returns>
    public static Result<T> ValidationFailure(List<string> errors)
    {
        return new Result<T>(default, false, ValidationErrorMessage, errors, StatusCodes.BadRequest);
    }
}

/// <summary>
/// Common status codes used by application results without referencing ASP.NET Core.
/// </summary>
public static class StatusCodes
{
    /// <summary>
    /// HTTP 200 OK.
    /// </summary>
    public const int Ok = 200;

    /// <summary>
    /// HTTP 201 Created.
    /// </summary>
    public const int Created = 201;

    /// <summary>
    /// HTTP 204 No Content.
    /// </summary>
    public const int NoContent = 204;

    /// <summary>
    /// HTTP 400 Bad Request.
    /// </summary>
    public const int BadRequest = 400;

    /// <summary>
    /// HTTP 401 Unauthorized.
    /// </summary>
    public const int Unauthorized = 401;

    /// <summary>
    /// HTTP 403 Forbidden.
    /// </summary>
    public const int Forbidden = 403;

    /// <summary>
    /// HTTP 404 Not Found.
    /// </summary>
    public const int NotFound = 404;

    /// <summary>
    /// HTTP 422 Unprocessable Entity.
    /// </summary>
    public const int UnprocessableEntity = 422;

    /// <summary>
    /// HTTP 500 Internal Server Error.
    /// </summary>
    public const int InternalServerError = 500;
}
