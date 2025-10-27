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
    


    
}