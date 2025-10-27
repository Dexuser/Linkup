using AutoMapper;
using LinkUp.Core.Application.Dtos.FriendShipRequest;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class FriendShipRequestMappingProfile : Profile
{
    public FriendShipRequestMappingProfile()
    {
        CreateMap<FriendShipRequest, FriendShipRequestDto>().ReverseMap();
    }
}