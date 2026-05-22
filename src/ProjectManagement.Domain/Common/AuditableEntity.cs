namespace ProjectManagement.Domain.Common;

public abstract class AuditableEntity : BaseEntity, IAuditableEntity, ISoftDelete
{
    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }

    public void MarkAsDeleted(DateTimeOffset deletedAt)
    {
        if (IsDeleted)
        {
            return;
        }

        IsDeleted = true;
        DeletedAt = deletedAt;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }
}
