using Domain.Entities.Teams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class PsychologistReviewConfiguration : IEntityTypeConfiguration<PsychologistReview>
{
    public void Configure(EntityTypeBuilder<PsychologistReview> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Comment).IsRequired(false);
        
        builder.Property(r => r.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(r => r.ReviewedAt).IsRequired(false);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.HasOne(r => r.Team)
            .WithMany(t => t.PsychologistReviews)
            .HasForeignKey(r => r.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Psychologist)
            .WithMany()
            .HasForeignKey(r => r.PsychologistId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
