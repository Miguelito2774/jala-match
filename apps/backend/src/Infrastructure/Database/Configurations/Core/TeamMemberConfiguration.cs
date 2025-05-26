using System.Text.Json;
using Domain.Entities.Teams;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class TeamMemberConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(tm => new { tm.TeamId, tm.EmployeeProfileId });

        builder.HasOne(tm => tm.Team).WithMany(t => t.Members).HasForeignKey(tm => tm.TeamId);

        builder
            .HasOne(tm => tm.EmployeeProfile)
            .WithMany(ep => ep.TeamMemberships)
            .HasForeignKey(tm => tm.EmployeeProfileId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(tm => tm.Role).HasConversion<string>().HasMaxLength(50);
    }
}
