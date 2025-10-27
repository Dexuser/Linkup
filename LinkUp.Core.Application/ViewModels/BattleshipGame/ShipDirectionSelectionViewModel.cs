using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.ViewModels.BattleshipGame;

public class ShipDirectionSelectionViewModel
{
    public required int GameId { get; set; }
    public required int X { get; set; }
    public required int Y { get; set; }
    public ShipType SelectedShip { get; set; }
    public Direction SelectedDirection { get; set; }
}