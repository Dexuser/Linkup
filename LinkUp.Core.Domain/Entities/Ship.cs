using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Domain.Entities;

public class Ship
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
    public BattleshipGame Game { get; set; }
    public ICollection<ShipPosition> Positions { get; set; }
}
