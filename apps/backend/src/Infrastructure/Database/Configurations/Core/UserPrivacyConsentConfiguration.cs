using Domain.Entities.Privacy;
using Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class UserPrivacyConsentConfiguration : EntityConfiguration<UserPrivacyConsent>
{
    protected override void ConfigureEntity(EntityTypeBuilder<UserPrivacyConsent> builder)
    {
        builder.HasIndex(c => c.UserId).IsUnique();

        builder.Property(c => c.Version)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.LastUpdated)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Relationship with User
        builder.HasOne(c => c.User)
            .WithOne(u => u.PrivacyConsent)
            .HasForeignKey<UserPrivacyConsent>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
