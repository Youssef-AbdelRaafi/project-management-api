namespace ProjectManagement.Domain.Common;

/// <summary>
/// Represents an entity that is hidden from normal queries instead of physically deleted.
/// </summary>
public interface ISoftDelete
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }

    /// <summary>
    /// Marks the entity as deleted.
    /// </summary>
    /// <param name="deletedAt">The UTC deletion timestamp.</param>
    void MarkAsDeleted(DateTimeOffset deletedAt);
}
