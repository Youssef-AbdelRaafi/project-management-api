namespace ProjectManagement.Domain.Exceptions;

/// <summary>
/// Represents a business rule violation in the domain model.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException" /> class.
    /// </summary>
    /// <param name="message">The business error message.</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainException" /> class.
    /// </summary>
    /// <param name="message">The business error message.</param>
    /// <param name="innerException">The underlying exception.</param>
    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
