using Domain.Entities.Privacy;
using Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class DataDeletionRequestConfiguration : EntityConfiguration<DataDeletionOrder>
{
    protected override void ConfigureEntity(EntityTypeBuilder<DataDeletionOrder> builder)
    {
        builder.HasIndex(r => r.UserId);

        builder.Property(r => r.Status).HasConversion<int>().IsRequired();

        builder.Property(r => r.RequestDate).IsRequired();

        builder
            .Property(r => r.DataTypes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            )
            .HasMaxLength(1000);

        builder
            .Property(r => r.DataTypes)
            .Metadata.SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );

        builder.Property(r => r.Reason).HasMaxLength(1000);

        builder.Property(r => r.CancellationReason).HasMaxLength(1000);

        // Relationship with User
        builder
            .HasOne(r => r.User)
            .WithMany(u => u.DataDeletionRequests)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
