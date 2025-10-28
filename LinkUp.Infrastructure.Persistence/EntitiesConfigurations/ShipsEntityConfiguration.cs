using LinkUp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class ShipsEntityConfiguration : IEntityTypeConfiguration<Ship>
{
    public void Configure(EntityTypeBuilder<Ship> builder)
    {
        builder.HasKey(s => s.Id);

        // Configuraciones de propiedades
        builder.Property(s => s.GameId)
            .IsRequired();

        builder.Property(s => s.PlayerId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(s => s.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.Direction)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(s => s.Size)
            .IsRequired();

        builder.Property(s => s.StartX)
            .IsRequired();

        builder.Property(s => s.StartY)
            .IsRequired();

        builder.Property(s => s.IsSunk)
            .IsRequired()
            .HasDefaultValue(false);

        // Relaciones internas
        builder.HasOne(s => s.Game)
            .WithMany(g => g.Ships)
            .HasForeignKey(s => s.GameId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasMany(s => s.Positions)
            .WithOne(sp => sp.Ship)
            .HasForeignKey(sp => sp.ShipId)
            .OnDelete(DeleteBehavior.Cascade);


        builder.HasIndex(s => new { s.GameId, s.PlayerId });
        builder.HasIndex(s => s.IsSunk);
    }
}