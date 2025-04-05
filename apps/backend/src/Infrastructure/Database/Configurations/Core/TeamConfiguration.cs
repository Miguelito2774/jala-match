using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
        
        builder.HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTeams)
            .HasForeignKey(t => t.CreatorId);
        
        builder.Property(t => t.RequiredTechnologies).HasColumnType("jsonb");
        
        builder.Property(t => t.Members).HasColumnType("jsonb");
        
        builder.Property(t => t.AiAnalysis).HasColumnType("jsonb");
    }
}
