using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.FriendShipRequest;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Application.Dtos.User;

namespace LinkUp.Core.Application.Interfaces;

public interface IFriendShipRequestService : IGenericService<FriendShipRequestDto>
{
    Task<Result> RespondRequestAsync(int friendShipRequestId, bool accept);
    Task<Result<List<FriendShipRequestDto>>> GetAllTheRequestsOfThisUser(string userId);
}