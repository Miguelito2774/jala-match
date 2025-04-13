using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class EmployeeLanguageConfiguration : IEntityTypeConfiguration<EmployeeLanguage>
{
    public void Configure(EntityTypeBuilder<EmployeeLanguage> builder)
    {
        builder
            .HasOne(el => el.EmployeeProfile)
            .WithMany(ep => ep.Languages)
            .HasForeignKey(el => el.EmployeeProfileId);

        builder.Property(el => el.Language).HasMaxLength(50).IsRequired();
        builder.Property(el => el.Proficiency).HasMaxLength(20).IsRequired();
    }
}
