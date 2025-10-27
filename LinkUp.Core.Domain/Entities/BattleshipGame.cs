using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Domain.Entities;


public class BattleshipGame
{
    public int Id { get; set; }
    
    // Jugadores
    public string Player1Id { get; set; }
    public string Player2Id { get; set; }
    
    // Estado del juego
    public GameStatus Status { get; set; } = GameStatus.SettingUp;
    public string CurrentTurnPlayerId { get; set; }
    public string WinnerId { get; set; }
    
    // Tiempos
    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }
    public DateTime LastMoveDate { get; set; } = DateTime.Now;
    
    // Relaciones
    public ICollection<Ship> Ships { get; set; }
    public ICollection<Attack> Attacks { get; set; }
}










