namespace ProjectManagement.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }

    DateTimeOffset? DeletedAt { get; set; }

    void MarkAsDeleted(DateTimeOffset deletedAt);
}
