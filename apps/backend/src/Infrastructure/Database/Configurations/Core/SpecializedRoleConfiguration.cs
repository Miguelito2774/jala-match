using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

public class SpecializedRoleConfiguration : IEntityTypeConfiguration<SpecializedRole>
{
    public void Configure(EntityTypeBuilder<SpecializedRole> builder)
    {
        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(s => new { s.TechnicalAreaId, s.Name }).IsUnique();
        
        builder.HasOne(s => s.TechnicalArea)
            .WithMany(t => t.Roles)
            .HasForeignKey(s => s.TechnicalAreaId);
    }
}

