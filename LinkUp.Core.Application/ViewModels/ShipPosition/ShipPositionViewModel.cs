using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.ViewModels.Ship;

namespace LinkUp.Core.Application.ViewModels.ShipPosition;

public class ShipPositionViewModel
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsHit { get; set; } = false;
    
    // Relaciones
    public ShipViewModel Ship { get; set; }
}
