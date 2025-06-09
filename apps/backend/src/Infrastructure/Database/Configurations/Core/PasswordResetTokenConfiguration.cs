using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
    {
        builder.Property(p => p.Token).IsRequired().HasMaxLength(50);

        builder.Property(p => p.Email).IsRequired().HasMaxLength(255);

        builder.Property(p => p.ExpiresAt).IsRequired();

        builder.Property(p => p.IsUsed).IsRequired();

        builder
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.Token).IsUnique();
        builder.HasIndex(p => p.Email);
    }
}
