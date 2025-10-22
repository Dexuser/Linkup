using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.FriendShipRequest;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;

namespace LinkUp.Core.Application.Services;

public class FriendShipRequestService : GenericServices<FriendShipRequest, FriendShipRequestDto>, IFriendShipRequestService 
{
    public FriendShipRequestService(IFriendShipRequestRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}