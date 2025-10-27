using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Application.ViewModels.Attack;
using LinkUp.Core.Application.ViewModels.Ship;
using LinkUp.Core.Application.ViewModels.User;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.ViewModels.BattleshipGame;


public class BattleshipGameViewModel
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
    public UserViewModel? Player1 { get; set; } // se carga en el servicio
    public UserViewModel? Player2 { get; set; }
    public ICollection<ShipViewModel> Ships { get; set; }
    public ICollection<AttackViewModel> Attacks { get; set; }
}










