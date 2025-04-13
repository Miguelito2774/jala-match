using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.CompatibilityScore).HasPrecision(5, 2);
        builder.Property(t => t.AiAnalysis).HasColumnType("jsonb");
        builder.Property(t => t.WeightCriteria).HasColumnType("jsonb");

        builder
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTeams)
            .HasForeignKey(t => t.CreatorId);
    }
}
