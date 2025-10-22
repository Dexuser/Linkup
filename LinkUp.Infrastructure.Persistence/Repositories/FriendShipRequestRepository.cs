using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using LinkUp.Core.Persistence.Context;

namespace LinkUp.Core.Persistence.Repositories;

public class FriendShipRequestRepository : GenericRepository<FriendShipRequest>, IFriendShipRequestRepository 
{
    public FriendShipRequestRepository(LinkUpContext context) : base(context)
    {
    }
}