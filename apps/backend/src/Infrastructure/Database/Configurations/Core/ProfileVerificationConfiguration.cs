using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class ProfileVerificationConfiguration : IEntityTypeConfiguration<ProfileVerification>
{
    public void Configure(EntityTypeBuilder<ProfileVerification> builder)
    {
        builder.HasOne(pv => pv.EmployeeProfile)
            .WithMany(ep => ep.Verifications)
            .HasForeignKey(pv => pv.EmployeeProfileId);
        
        builder.HasOne(pv => pv.Reviewer)
            .WithMany(u => u.Reviews)
            .HasForeignKey(pv => pv.ReviewerId);
        
        builder.Property(pv => pv.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
    }
}
