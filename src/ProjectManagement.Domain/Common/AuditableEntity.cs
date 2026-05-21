namespace ProjectManagement.Domain.Common;

/// <summary>
/// Base type for entities that require auditing and soft-delete support.
/// </summary>
public abstract class AuditableEntity : BaseEntity, IAuditableEntity, ISoftDelete
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public string? CreatedBy { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <inheritdoc />
    public string? UpdatedBy { get; set; }

    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? DeletedAt { get; set; }

    /// <inheritdoc />
    public void MarkAsDeleted(DateTimeOffset deletedAt)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = deletedAt;
    }

    /// <summary>
    /// Restores a previously soft-deleted entity.
    /// </summary>
    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
