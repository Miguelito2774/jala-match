using Domain.Entities.Technologies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class TechnologyConfiguration : IEntityTypeConfiguration<Technology>
{
    public void Configure(EntityTypeBuilder<Technology> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        
        builder.Property(t => t.Version).HasMaxLength(20);
        
        builder.HasOne(t => t.Category)
            .WithMany(tc => tc.Technologies)
            .HasForeignKey(t => t.CategoryId);
    }
}
