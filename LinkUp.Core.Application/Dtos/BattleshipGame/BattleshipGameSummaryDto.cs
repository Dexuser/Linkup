using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.Dtos.BattleshipGame;


public class BattleshipGameSummaryDto
{
    public int GamesCount { get; set; }
    public int GamesLoseCount { get; set; }
    public int GamesWinCount { get; set; }
}










