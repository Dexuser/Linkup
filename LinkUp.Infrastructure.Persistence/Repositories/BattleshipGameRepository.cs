using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class BattleshipGameRepository : GenericRepository<BattleshipGame>, IBattleshipGameRepository
{
    public BattleshipGameRepository(LinkUpContext context) : base(context)
    {
    }

    public async Task<bool> SetStatus(int gameId, GameStatus status)
    {
        var game = await Context.Set<BattleshipGame>().FindAsync(gameId);
        if (game != null)
        {
            game.Status = status;
            await Context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> ChangeTurn(int gameId)
    {
        var game = await Context.Set<BattleshipGame>().FindAsync(gameId);
        if (game != null)
        {
            if (game.CurrentTurnPlayerId == game.Player1Id)
            {
                game.CurrentTurnPlayerId = game.Player2Id;
            }
            else
            {
                game.CurrentTurnPlayerId = game.Player1Id;
            }
            await Context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task UpdateLastMove(int gameId)
    {
        var game = await Context.Set<BattleshipGame>().FindAsync(gameId);
        if (game != null)
        {
            game.LastMoveDate = DateTime.Now;
        }
    }
}