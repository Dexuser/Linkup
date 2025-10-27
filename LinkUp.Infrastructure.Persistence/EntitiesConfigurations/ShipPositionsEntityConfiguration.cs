using LinkUp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class ShipPositionsEntityConfiguration : IEntityTypeConfiguration<ShipPosition>
{
    public void Configure(EntityTypeBuilder<ShipPosition> builder)
    {
        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.ShipId)
            .IsRequired();

        builder.Property(sp => sp.X)
            .IsRequired();

        builder.Property(sp => sp.Y)
            .IsRequired();

        builder.Property(sp => sp.IsHit)
            .IsRequired()
            .HasDefaultValue(false);

        // Relación interna
        builder.HasOne(sp => sp.Ship)
            .WithMany(s => s.Positions)
            .HasForeignKey(sp => sp.ShipId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Índices
        builder.HasIndex(sp => new { sp.ShipId, sp.X, sp.Y })
            .IsUnique();

        builder.HasIndex(sp => new { sp.ShipId, sp.IsHit });
        builder.HasIndex(sp => new { sp.X, sp.Y });
    }
}