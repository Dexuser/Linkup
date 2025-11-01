using LinkUp.Core.Application.Dtos.BattleshipGame;

namespace LinkUp.Core.Application.ViewModels.BattleshipGame;

public class BattleshipGameHomeIndex
{
    public List<BattleshipGameViewModel> ActiveGames { get; set; } = [];
    public List<BattleshipGameViewModel> FinalizedGames{ get; set; } = [];
    public required BattleshipGameSummaryViewModel Summary { get; set; }
}