using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Entities;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(user => user.FullName)
            .IsRequired()
            .HasMaxLength(DomainConstants.User.FullNameMaxLength);
    }
}
