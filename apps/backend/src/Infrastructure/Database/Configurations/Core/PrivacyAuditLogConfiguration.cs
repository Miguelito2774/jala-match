using Domain.Entities.Privacy;
using Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class PrivacyAuditLogConfiguration : EntityConfiguration<PrivacyAuditLog>
{
    protected override void ConfigureEntity(EntityTypeBuilder<PrivacyAuditLog> builder)
    {
        builder.HasIndex(l => l.UserId);
        builder.HasIndex(l => l.Timestamp);

        builder.Property(l => l.Action)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(l => l.Details)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(l => l.Timestamp)
            .IsRequired();

        builder.Property(l => l.IpAddress)
            .HasMaxLength(45); // IPv6 max length

        builder.Property(l => l.UserAgent)
            .HasMaxLength(500);

        // Relationship with User
        builder.HasOne(l => l.User)
            .WithMany(u => u.PrivacyAuditLogs)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
