using LinkUp.Core.Persistence.Common;
using LinkUp.Core.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class FriendshipRequestEntityConfiguration : IEntityTypeConfiguration<FriendshipRequest>
{
    public void Configure(EntityTypeBuilder<FriendshipRequest> builder)
    {
        builder.HasKey(f => f.Id);
        builder.Property(f => f.RequestedByUserId).HasMaxLength(450).IsRequired();
        builder.Property(f => f.TargetUserId).HasMaxLength(450).IsRequired();
        builder.Property(f => f.CreatedAt).IsRequired();
        
        builder.Property(f => f.Status).HasConversion<string>()
            .HasDefaultValue(FriendShipRequestStatus.Pending)
            .IsRequired();
    }
}