using LinkUp.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Core.Persistence.EntitiesConfigurations;

public class BattleshipGameEntityConfigurations : IEntityTypeConfiguration<BattleshipGame>
{
    public void Configure(EntityTypeBuilder<BattleshipGame> builder)
    {
            builder.HasKey(g => g.Id);
        
            // Configuraciones de propiedades (sin relaciones a Identity)
            builder.Property(g => g.Player1Id)
                .IsRequired()
                .HasMaxLength(450); // Típico tamaño de ID en Identity

            builder.Property(g => g.Player2Id)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(g => g.CurrentTurnPlayerId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(g => g.WinnerId)
                .HasMaxLength(450)
                .IsRequired(false);

            builder.Property(g => g.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(g => g.StartDate)
                .IsRequired();

            builder.Property(g => g.LastMoveDate)
                .IsRequired();

            // Relaciones internas del módulo Battleship
            builder.HasMany(g => g.Ships)
                .WithOne(s => s.Game)
                .HasForeignKey(s => s.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Attacks)
                .WithOne(a => a.Game)
                .HasForeignKey(a => a.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Índices para mejor performance
            builder.HasIndex(g => g.Status);
            builder.HasIndex(g => g.Player1Id);
            builder.HasIndex(g => g.Player2Id);
            builder.HasIndex(g => g.CurrentTurnPlayerId);
            builder.HasIndex(g => g.LastMoveDate);
    }
}