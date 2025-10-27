using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Persistence.Repositories;

public class FriendShipRequestRepository : GenericRepository<FriendShipRequest>, IFriendShipRequestRepository 
{
    public FriendShipRequestRepository(LinkUpContext context) : base(context)
    {
    }
    public async Task RespondRequestAsync(int friendShipRequestId, bool accept)
    {
        var request = Context.Set<FriendShipRequest>().FirstOrDefault(x => x.Id == friendShipRequestId);
        if (request != null)
        {
            request.Status = accept ? FriendShipRequestStatus.Accepted : FriendShipRequestStatus.Rejected;
            await Context.SaveChangesAsync();
        }
    }
}