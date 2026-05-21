namespace ProjectManagement.Domain.Common;

/// <summary>
/// Represents an entity whose lifecycle is tracked by the persistence layer.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user that created the entity.
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was last updated.
    /// </summary>
    DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user that last updated the entity.
    /// </summary>
    string? UpdatedBy { get; set; }
}
