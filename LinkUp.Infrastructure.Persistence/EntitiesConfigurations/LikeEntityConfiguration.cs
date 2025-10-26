using LinkUp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class LikeEntityConfiguration : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.PostId).IsRequired();
        builder.Property(l => l.UserId).HasMaxLength(450).IsRequired();
        builder.Property(l => l.IsLiked).HasDefaultValue(null);
    }
}