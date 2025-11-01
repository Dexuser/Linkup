using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Domain.Interfaces;

public interface IFriendShipRepository : IGenericRepository<FriendShip>
{
    Task<List<string>> GetAllTheFriendsIds(string userId);
    Task DeleteFriendShip(string userId1, string userId2);
}