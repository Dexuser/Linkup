using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Persistence.Repositories;

public class FriendShipRepository : GenericRepository<FriendShip>, IFriendShipRepository
{
    public FriendShipRepository(LinkUpContext context) : base(context)
    {
    }
    public async Task<List<string>> GetAllTheFriendsIds(string userId)
    {
        var friendsIds = await Context.Set<FriendShip>()
            .AsNoTracking()
            .Where(f => f.UserId1 == userId || f.UserId2 == userId)
            .Select(f => (f.UserId1 == userId) ? f.UserId2: f.UserId1)
            .ToListAsync();

        return friendsIds;
    }

    public async Task DeleteFriendShip(string userId1, string userId2)
    {
        var frienship = await Context.Set<FriendShip>()
            .FirstOrDefaultAsync(f =>
                f.UserId1 == userId1 && f.UserId2 == userId2
                ||
                f.UserId1 == userId2 && f.UserId2 == userId1);
        
        if (frienship != null)
        {
            Context.Remove(frienship);
            await Context.SaveChangesAsync();
        }
    }
}