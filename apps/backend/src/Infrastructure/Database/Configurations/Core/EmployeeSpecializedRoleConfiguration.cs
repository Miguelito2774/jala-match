using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

public class EmployeeSpecializedRoleConfiguration
    : IEntityTypeConfiguration<EmployeeSpecializedRole>
{
    public void Configure(EntityTypeBuilder<EmployeeSpecializedRole> builder)
    {
        builder.HasKey(es => new { es.EmployeeProfileId, es.SpecializedRoleId });

        builder
            .HasOne(es => es.EmployeeProfile)
            .WithMany(e => e.SpecializedRoles)
            .HasForeignKey(es => es.EmployeeProfileId);

        builder
            .HasOne(es => es.SpecializedRole)
            .WithMany()
            .HasForeignKey(es => es.SpecializedRoleId);

        builder.Property(es => es.Level).HasConversion<string>().HasMaxLength(20).IsRequired();
    }
}
