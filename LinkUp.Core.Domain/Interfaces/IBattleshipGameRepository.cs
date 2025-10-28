using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Domain.Interfaces;

public interface IBattleshipGameRepository : IGenericRepository<BattleshipGame>
{
    Task<bool> SetStatus(int gameId, GameStatus status);
    Task<bool> ChangeTurn(int gameId);
    Task UpdateLastMove(int gameId);
}