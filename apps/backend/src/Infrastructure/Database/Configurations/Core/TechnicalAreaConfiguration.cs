using Domain.Entities.Areas_Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

public class TechnicalAreaConfiguration : IEntityTypeConfiguration<TechnicalArea>
{
    public void Configure(EntityTypeBuilder<TechnicalArea> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(t => t.Name).IsUnique();
    }
}

