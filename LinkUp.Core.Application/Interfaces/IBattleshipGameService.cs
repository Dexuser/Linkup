using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.User;

namespace LinkUp.Core.Application.Interfaces;

public interface IBattleshipGameService : IGenericService<BattleshipGameDto>
{
    Task<Result<List<BattleshipGameDto>>> GetAllBattleshipGamesOfUser(string userId);
    Task<Result<List<UserDto>>> GetAllTheUsersAvailableForAGame(string userId, string? userNameFilter = null);
}