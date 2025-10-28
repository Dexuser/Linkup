using LinkUp.Core.Application.Dtos.BattleshipGame;

namespace LinkUp.Core.Application.Dtos.Attack;

public class AttackDto
{
    public int Id { get; set; }
    
    // Relación con juego y atacante
    public required int GameId { get; set; }
    public required string AttackerId { get; set; }
    
    // Coordenadas del ataque
    public required int X { get; set; }
    public required int Y { get; set; }
    
    // Resultado
    public bool IsHit { get; set; }
    public DateTime AttackTime { get; set; } = DateTime.Now;
    
    // Relaciones
    public BattleshipGameDto Game { get; set; }
}
