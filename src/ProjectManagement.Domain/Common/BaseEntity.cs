namespace ProjectManagement.Domain.Common;

/// <summary>
/// Base type for aggregate entities that use a GUID identity.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Initializes a new entity with an application-generated identifier.
    /// </summary>
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets the entity identifier.
    /// </summary>
    public Guid Id { get; protected set; }
}
