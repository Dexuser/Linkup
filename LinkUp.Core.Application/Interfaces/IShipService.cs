using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.Interfaces;

public interface IShipService : IGenericService<ShipDto>
{
    Task<Result<List<ShipType>>> GetMissingShips(string userId, int gameId);
    Task<Result> CheckIfShipSunk(int shipId);
}