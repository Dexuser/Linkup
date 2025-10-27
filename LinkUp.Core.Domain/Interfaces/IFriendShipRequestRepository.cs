using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Domain.Interfaces;

public interface IFriendShipRequestRepository : IGenericRepository<FriendShipRequest>
{
    Task RespondRequestAsync(int friendShipRequestId, bool accept);
}