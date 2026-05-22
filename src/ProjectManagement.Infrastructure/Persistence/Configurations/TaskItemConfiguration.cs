using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configures persistence rules for task items.
/// </summary>
public sealed class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("TaskItems");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .IsRequired()
            .HasMaxLength(DomainConstants.TaskItem.TitleMaxLength);

        builder.Property(task => task.Description)
            .HasMaxLength(DomainConstants.TaskItem.DescriptionMaxLength);

        builder.Property(task => task.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(task => task.Priority)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(task => task.DueDate)
            .IsRequired();

        builder.Property(task => task.ProjectId)
            .IsRequired();

        builder.HasIndex(task => task.ProjectId);
        builder.HasIndex(task => task.Status);
        builder.HasIndex(task => task.IsDeleted);
    }
}
