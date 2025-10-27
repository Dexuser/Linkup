using LinkUp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class FriendShipEntityConfiguration : IEntityTypeConfiguration<FriendShip>
{
    public void Configure(EntityTypeBuilder<FriendShip> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UserId1).IsRequired().HasMaxLength(450); 
        builder.Property(x => x.UserId2).IsRequired().HasMaxLength(450);
    }
}