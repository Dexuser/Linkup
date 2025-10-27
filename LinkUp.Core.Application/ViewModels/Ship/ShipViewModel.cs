using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Application.ViewModels.BattleshipGame;
using LinkUp.Core.Application.ViewModels.ShipPosition;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.ViewModels.Ship;

public class ShipViewModel
{
    public int Id { get; set; }
    
    // Relación con juego y jugador
    public int GameId { get; set; }
    public string PlayerId { get; set; }
    
    // Tipo y tamaño del barco
    public ShipType Type { get; set; }
    public int Size { get; set; }
    
    // Posición y dirección
    public int StartX { get; set; }
    public int StartY { get; set; }
    public Direction Direction { get; set; }
    
    // Estado
    public bool IsSunk { get; set; } = false;
    
    // Relaciones
    public BattleshipGameViewModel Game { get; set; }
    public ICollection<ShipPositionViewModel> Positions { get; set; }
}
