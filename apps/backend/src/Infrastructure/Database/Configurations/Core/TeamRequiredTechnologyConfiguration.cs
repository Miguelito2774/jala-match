using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class TeamRequiredTechnologyConfiguration
    : IEntityTypeConfiguration<TeamRequiredTechnology>
{
    public void Configure(EntityTypeBuilder<TeamRequiredTechnology> builder)
    {
        builder.HasKey(trt => new { trt.TeamId, trt.TechnologyId });

        builder
            .HasOne(trt => trt.Team)
            .WithMany(t => t.RequiredTechnologies)
            .HasForeignKey(trt => trt.TeamId);

        builder
            .HasOne(trt => trt.Technology)
            .WithMany(t => t.TeamRequiredTechnologies as IEnumerable<TeamRequiredTechnology>)
            .HasForeignKey(trt => trt.TechnologyId);

        builder.Property(trt => trt.MinimumSfiaLevel).HasDefaultValue(3);
    }
}
