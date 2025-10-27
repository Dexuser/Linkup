using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.ViewModels.BattleshipGame;

public class ShipSelectionViewModel
{
    public required int GameId { get; set; }
    public List<ShipType> MissingShips { get; set; } = [];
    public int SelectedShip { get; set; }
}