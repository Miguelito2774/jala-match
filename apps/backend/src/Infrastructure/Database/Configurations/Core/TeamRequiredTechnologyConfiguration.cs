using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

public class TeamRequiredTechnologyConfiguration : IEntityTypeConfiguration<TeamRequiredTechnology>
{
    public void Configure(EntityTypeBuilder<TeamRequiredTechnology> builder)
    {
        builder.HasKey(rt => new { rt.TeamId, rt.TechnologyId });

        builder
            .HasOne(rt => rt.Team)
            .WithMany(t => t.RequiredTechnologies)
            .HasForeignKey(rt => rt.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(rt => rt.Technology)
            .WithMany(t => t.TeamRequiredTechnologies)
            .HasForeignKey(rt => rt.TechnologyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
