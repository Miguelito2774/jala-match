using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

public class SpecializedRoleSkillConfiguration : IEntityTypeConfiguration<SpecializedRoleSkill>
{
    public void Configure(EntityTypeBuilder<SpecializedRoleSkill> builder)
    {
        builder.HasKey(rs => new { rs.SpecializedRoleId, rs.TechnologyId });
        
        builder.HasOne(rs => rs.SpecializedRole)
            .WithMany(s => s.RequiredSkills)
            .HasForeignKey(rs => rs.SpecializedRoleId);

        builder.HasOne(rs => rs.Technology)
            .WithMany()
            .HasForeignKey(rs => rs.TechnologyId);
    }
}
