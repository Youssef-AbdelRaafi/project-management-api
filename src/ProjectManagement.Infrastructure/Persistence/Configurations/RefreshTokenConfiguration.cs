using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Domain.Common;
using ProjectManagement.Domain.Entities;
using ProjectManagement.Infrastructure.Identity;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.TokenHash)
            .IsRequired()
            .HasMaxLength(DomainConstants.RefreshToken.TokenHashMaxLength);

        builder.Property(refreshToken => refreshToken.UserId)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedByIp)
            .HasMaxLength(DomainConstants.RefreshToken.IpAddressMaxLength);

        builder.Property(refreshToken => refreshToken.RevokedByIp)
            .HasMaxLength(DomainConstants.RefreshToken.IpAddressMaxLength);

        builder.Property(refreshToken => refreshToken.ReplacedByTokenHash)
            .HasMaxLength(DomainConstants.RefreshToken.TokenHashMaxLength);

        builder.Property(refreshToken => refreshToken.RevokedReason)
            .HasMaxLength(DomainConstants.RefreshToken.RevokedReasonMaxLength);

        builder.HasIndex(refreshToken => refreshToken.TokenHash)
            .IsUnique();

        builder.HasIndex(refreshToken => refreshToken.UserId);
        builder.HasIndex(refreshToken => refreshToken.ExpiresAt);

        builder.HasOne<ApplicationUser>()
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
