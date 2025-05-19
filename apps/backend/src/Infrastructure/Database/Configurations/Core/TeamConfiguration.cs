using System.Text.Json;
using Application.DTOs;
using Domain.Entities.Teams;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
        builder.Property(t => t.CompatibilityScore).HasPrecision(5, 2);
        builder.Property(t => t.AiAnalysis).HasColumnType("jsonb");
        builder.Property(t => t.WeightCriteria).HasColumnType("jsonb");

        builder
            .HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTeams)
            .HasForeignKey(t => t.CreatorId);

        builder
            .HasMany(t => t.Members)
            .WithOne(m => m.Team)
            .HasForeignKey(m => m.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(t => t.RequiredTechnologies)
            .WithOne(rt => rt.Team)
            .HasForeignKey(rt => rt.TeamId);

        builder.Ignore(t => t.TeamSize);
    }
}
