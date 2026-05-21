namespace ProjectManagement.Domain.Exceptions;

/// <summary>
/// Represents a missing domain resource.
/// </summary>
public sealed class NotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException" /> class.
    /// </summary>
    /// <param name="resourceName">The missing resource name.</param>
    /// <param name="resourceKey">The missing resource identifier.</param>
    public NotFoundException(string resourceName, object resourceKey)
        : base($"{resourceName} with identifier '{resourceKey}' was not found.")
    {
        ResourceName = resourceName;
        ResourceKey = resourceKey;
    }

    /// <summary>
    /// Gets the missing resource name.
    /// </summary>
    public string ResourceName { get; }

    /// <summary>
    /// Gets the missing resource identifier.
    /// </summary>
    public object ResourceKey { get; }
}
