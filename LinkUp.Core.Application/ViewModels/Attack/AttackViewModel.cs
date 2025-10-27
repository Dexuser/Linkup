using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.ViewModels.BattleshipGame;

namespace LinkUp.Core.Application.ViewModels.Attack;

public class AttackViewModel
{
    public int Id { get; set; }
    
    // Relación con juego y atacante
    public int GameId { get; set; }
    public string AttackerId { get; set; }
    
    // Coordenadas del ataque
    public int X { get; set; }
    public int Y { get; set; }
    
    // Resultado
    public bool IsHit { get; set; }
    public DateTime AttackTime { get; set; } = DateTime.Now;
    
    // Relaciones
    public BattleshipGameViewModel Game { get; set; }
}
