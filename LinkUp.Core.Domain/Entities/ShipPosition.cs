namespace LinkUp.Core.Domain.Entities;

public class ShipPosition
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsHit { get; set; } = false;
    
    // Relaciones
    public Ship Ship { get; set; }
}
