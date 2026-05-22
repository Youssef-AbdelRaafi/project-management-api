using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Name)
            .IsRequired()
            .HasMaxLength(DomainConstants.Project.NameMaxLength);

        builder.Property(project => project.Description)
            .HasMaxLength(DomainConstants.Project.DescriptionMaxLength);

        builder.Property(project => project.UserId)
            .IsRequired();

        builder.HasIndex(project => project.UserId);
        builder.HasIndex(project => project.IsDeleted);

        builder.HasOne(project => project.User)
            .WithMany(user => user.Projects)
            .HasForeignKey(project => project.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(project => project.Tasks)
            .WithOne(task => task.Project)
            .HasForeignKey(task => task.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
