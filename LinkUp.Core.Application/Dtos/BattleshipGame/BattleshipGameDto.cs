using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.Dtos.BattleshipGame;


public class BattleshipGameDto
{
    public int Id { get; set; }
    
    // Jugadores
    public required string Player1Id { get; set; }
    public required string Player2Id { get; set; }
    
    // Estado del juego
    public GameStatus Status { get; set; } = GameStatus.SettingUp;
    public required string CurrentTurnPlayerId { get; set; }
    public string WinnerId { get; set; }
    
    // Tiempos
    public required DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime? EndDate { get; set; }
    public DateTime LastMoveDate { get; set; } = DateTime.Now;
    
    // Relaciones
    public UserDto? Player1 { get; set; } // se carga en el servicio
    public UserDto? Player2 { get; set; }
    public ICollection<ShipDto> Ships { get; set; }
    public ICollection<AttackDto> Attacks { get; set; }
}










