using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.Dtos.Ship;

public class ShipDto
{
    public int Id { get; set; }
    
    // Relación con juego y jugador
    public required int GameId { get; set; }
    public required string PlayerId { get; set; }
    
    // Tipo y tamaño del barco
    public required ShipType Type { get; set; }
    public required int Size { get; set; }
    
    // Posición y dirección
    public required int StartX { get; set; }
    public required int StartY { get; set; }
    public required Direction Direction { get; set; }
    
    // Estado
    public bool IsSunk { get; set; } = false;
    
    // Relaciones
    public BattleshipGameDto Game { get; set; }
    public ICollection<ShipPositionDto> Positions { get; set; }
}
