using Domain.Entities.Enums;
using Domain.Entities.Profiles;
using Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class EmployeeProfileConfiguration : EntityConfiguration<EmployeeProfile>
{
    protected override void ConfigureEntity(EntityTypeBuilder<EmployeeProfile> builder)
    {
        builder
            .HasOne(ep => ep.User)
            .WithOne(u => u.EmployeeProfile)
            .HasForeignKey<EmployeeProfile>(ep => ep.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ep => ep.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(ep => ep.LastName).HasMaxLength(100).IsRequired();
        builder.Property(ep => ep.Country).HasMaxLength(100).IsRequired();
        builder.Property(ep => ep.Timezone).HasMaxLength(50).IsRequired();
        builder.Property(ep => ep.Mbti).HasMaxLength(4).IsRequired();
        builder.Property(ep => ep.SfiaLevelGeneral).HasPrecision(3, 1).IsRequired();
        builder
            .Property(ep => ep.VerificationStatus)
            .HasConversion(
                v => ((int)v).ToString(System.Globalization.CultureInfo.InvariantCulture),
                v =>
                    (VerificationStatus)
                        int.Parse(v, System.Globalization.CultureInfo.InvariantCulture)
            )
            .HasMaxLength(2)
            .IsRequired();
    }
}
