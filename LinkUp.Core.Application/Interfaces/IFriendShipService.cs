using LinkUp.Core.Application.Dtos.FriendShip;
using LinkUp.Core.Application.Dtos.User;

namespace LinkUp.Core.Application.Interfaces;

public interface IFriendShipService : IGenericService<FriendShipDto>
{
    Task<int> GetCommonFriendsCountAsync(string userId1, string userId2);
    Task<Result<List<UserWithCommonFriendCountDto>>> GetAllTheUsersThatAreNotFriends(string userId);
    Task<Result<List<string>>> GetAllTheFriendsIds(string userId);
    Task<Result<List<UserDto>>> GetAllTheFriends(string userId);
    Task<Result> DeleteFrienship(string userId1, string userId2);
}