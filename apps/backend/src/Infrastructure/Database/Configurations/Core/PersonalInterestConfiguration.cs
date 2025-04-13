using Domain.Entities.Profiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class PersonalInterestConfiguration : IEntityTypeConfiguration<PersonalInterest>
{
    public void Configure(EntityTypeBuilder<PersonalInterest> builder)
    {
        builder
            .HasOne(pi => pi.EmployeeProfile)
            .WithMany(ep => ep.PersonalInterests)
            .HasForeignKey(pi => pi.EmployeeProfileId);

        builder.Property(pi => pi.Name).HasMaxLength(100).IsRequired();
        builder.Property(pi => pi.Frequency).HasMaxLength(50);
        builder.Property(pi => pi.InterestLevel).HasDefaultValue(3);
    }
}
