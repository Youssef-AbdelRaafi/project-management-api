namespace ProjectManagement.Application.Common.Models;

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

    public bool IsSuccess { get; }

    public string? Error { get; }

    public List<string> Errors { get; }

    public int StatusCode { get; }

    public static Result Success(int statusCode = StatusCodes.Ok)
    {
        return new Result(true, null, [], statusCode);
    }

    public static Result Failure(string error, int statusCode = StatusCodes.BadRequest)
    {
        return new Result(false, error, [error], statusCode);
    }

    public static Result ValidationFailure(List<string> errors)
    {
        return new Result(false, ValidationErrorMessage, errors, StatusCodes.BadRequest);
    }
}

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

    public bool IsSuccess { get; }

    public T? Data { get; }

    public string? Error { get; }

    public List<string> Errors { get; }

    public int StatusCode { get; }

    public static Result<T> Success(T data, int statusCode = StatusCodes.Ok)
    {
        return new Result<T>(data, true, null, [], statusCode);
    }

    public static Result<T> Failure(string error, int statusCode = StatusCodes.BadRequest)
    {
        return new Result<T>(default, false, error, [error], statusCode);
    }

    public static Result<T> ValidationFailure(List<string> errors)
    {
        return new Result<T>(default, false, ValidationErrorMessage, errors, StatusCodes.BadRequest);
    }
}

public static class StatusCodes
{
    public const int Ok = 200;

    public const int Created = 201;

    public const int NoContent = 204;

    public const int BadRequest = 400;

    public const int Unauthorized = 401;

    public const int Forbidden = 403;

    public const int NotFound = 404;

    public const int UnprocessableEntity = 422;

    public const int InternalServerError = 500;
}
