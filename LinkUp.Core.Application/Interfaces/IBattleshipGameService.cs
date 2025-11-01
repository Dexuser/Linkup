using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.Interfaces;

public interface IBattleshipGameService : IGenericService<BattleshipGameDto>
{
    Task<Result<List<BattleshipGameDto>>> GetAllBattleshipGamesOfUser(string userId);
    Task<Result<List<UserDto>>> GetAllTheUsersAvailableForAGame(string userId, string? userNameFilter = null);
    Task<Result> SetStatus(int gameId, GameStatus status);
    Task<Result> ChangeTurn(int gameId);
    Task<Result<bool>>CheckIfGameEnded(int gameId);
    Task<Result> UpdateLastMove(int gameId);
    Task<Result> ThisUserGiveUp(int gameId, string userId);
    Task<Result<BattleshipGameSummaryDto>> GetSummaryOfThisUser(string userId);
}