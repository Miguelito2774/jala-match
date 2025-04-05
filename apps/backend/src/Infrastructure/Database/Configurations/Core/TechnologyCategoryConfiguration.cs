using Domain.Entities.Technologies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class TechnologyCategoryConfiguration : IEntityTypeConfiguration<TechnologyCategory>
{
    public void Configure(EntityTypeBuilder<TechnologyCategory> builder)
    {
        builder.Property(tc => tc.Name).HasMaxLength(50).IsRequired();
        
        builder.HasIndex(tc => tc.Name).IsUnique();
    }
}
