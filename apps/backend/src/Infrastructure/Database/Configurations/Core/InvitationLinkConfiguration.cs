using Domain.Entities.Invitations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configurations.Core;

internal sealed class InvitationLinkConfiguration : IEntityTypeConfiguration<InvitationLink>
{
    public void Configure(EntityTypeBuilder<InvitationLink> builder)
    {
        builder.Property(i => i.Token).IsRequired().HasMaxLength(50);

        builder.Property(i => i.Email).IsRequired().HasMaxLength(255);

        builder.Property(i => i.TargetRole).IsRequired().HasConversion<int>();

        builder
            .HasOne(i => i.CreatedBy)
            .WithMany()
            .HasForeignKey(i => i.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.Token).IsUnique();
    }
}
