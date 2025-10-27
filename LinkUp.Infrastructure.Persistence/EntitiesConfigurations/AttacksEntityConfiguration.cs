using LinkUp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class AttacksEntityConfiguration : IEntityTypeConfiguration<Attack>
{
    public void Configure(EntityTypeBuilder<Attack> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.GameId)
            .IsRequired();

        builder.Property(a => a.AttackerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(a => a.X)
            .IsRequired();

        builder.Property(a => a.Y)
            .IsRequired();

        builder.Property(a => a.IsHit)
            .IsRequired();

        builder.Property(a => a.AttackTime)
            .IsRequired();

        // Relación interna
        builder.HasOne(a => a.Game)
            .WithMany(g => g.Attacks)
            .HasForeignKey(a => a.GameId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Índices
        builder.HasIndex(a => new { a.GameId, a.AttackerId, a.X, a.Y })
            .IsUnique();

        builder.HasIndex(a => new { a.GameId, a.AttackerId });
        builder.HasIndex(a => new { a.GameId, a.X, a.Y });
        builder.HasIndex(a => a.AttackTime);
    }
}