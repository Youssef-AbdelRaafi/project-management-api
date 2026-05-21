namespace ProjectManagement.Domain.Exceptions;

/// <summary>
/// Represents a request that requires an authenticated user.
/// </summary>
public sealed class UnauthorizedException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedException" /> class.
    /// </summary>
    /// <param name="message">The authentication failure message.</param>
    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
