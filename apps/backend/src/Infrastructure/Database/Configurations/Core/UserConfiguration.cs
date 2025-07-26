using Domain.Entities.Users;
using Infrastructure.Database.Configurations.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class UserConfiguration : EntityConfiguration<User>
{
    protected override void ConfigureEntity(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Role).HasConversion<int>();
        
        builder.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
        
        builder.Property(u => u.ProfilePictureUrl).HasMaxLength(512);
        
        builder.Property(u => u.ProfilePicturePublicId).HasMaxLength(255);
        
    }
}
