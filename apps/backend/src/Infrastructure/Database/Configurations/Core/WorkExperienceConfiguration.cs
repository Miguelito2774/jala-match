using System.Text.Json;
using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder
            .HasOne(we => we.EmployeeProfile)
            .WithMany(ep => ep.WorkExperiences)
            .HasForeignKey(we => we.EmployeeProfileId);

        builder.Property(we => we.ProjectName).HasMaxLength(100).IsRequired();
        builder.Property(we => we.Description).HasMaxLength(1000);
        builder.Property(we => we.VersionControl).HasMaxLength(50);
        builder.Property(we => we.ProjectManagement).HasMaxLength(50);

        builder
            .Property(we => we.Tools)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            )
            .HasColumnType("jsonb")
            .Metadata.SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c =>
                        c.Aggregate(
                            0,
                            (a, v) => HashCode.Combine(a, v != null ? v.GetHashCode() : 0)
                        ),
                    c => c.ToList()
                )
            );

        builder
            .Property(we => we.Responsibilities)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null)
            )
            .HasColumnType("jsonb")
            .Metadata.SetValueComparer(
                new ValueComparer<List<string>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList()
                )
            );
    }
}
