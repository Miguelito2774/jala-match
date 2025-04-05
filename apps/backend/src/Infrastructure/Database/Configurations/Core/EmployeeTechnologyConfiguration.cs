using Domain.Entities.Technologies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class EmployeeTechnologyConfiguration : IEntityTypeConfiguration<EmployeeTechnology>
{
    public void Configure(EntityTypeBuilder<EmployeeTechnology> builder)
    {
        builder.HasKey(et => new { et.EmployeeProfileId, et.TechnologyId });
        
        builder.HasOne(et => et.EmployeeProfile)
            .WithMany(ep => ep.Technologies)
            .HasForeignKey(et => et.EmployeeProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(et => et.Technology)
            .WithMany(t => t.EmployeeTechnologies)
            .HasForeignKey(et => et.TechnologyId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Property(et => et.SfiaLevel).IsRequired();
        
        builder.Property(et => et.YearsExperience).HasColumnType("numeric(3,1)");
    }
}
