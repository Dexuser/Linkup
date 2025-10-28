using System.ComponentModel.DataAnnotations;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.ViewModels.BattleshipGame;

public class ShipSelectionViewModel
{
    public required int GameId { get; set; }
    public List<ShipType> MissingShips { get; set; } = [];
    
    [Required(ErrorMessage = "Debes de seleccionar un barco por lo menos")]
    [Range(2, 5, ErrorMessage = "debes de seleccionar un barco por lo menos")]
    public int SelectedShip { get; set; }
}