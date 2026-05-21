namespace ProjectManagement.Domain.Exceptions;

/// <summary>
/// Represents an authenticated user trying to access a resource they do not own.
/// </summary>
public sealed class ForbiddenException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenException" /> class.
    /// </summary>
    /// <param name="message">The authorization failure message.</param>
    public ForbiddenException(string message)
        : base(message)
    {
    }
}
