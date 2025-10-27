using AutoMapper;
using LinkUp.Core.Application.Dtos.FriendShip;
using LinkUp.Core.Application.Dtos.FriendShipRequest;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class FriendShipMappingProfile : Profile
{
    public FriendShipMappingProfile()
    {
        CreateMap<FriendShip, FriendShipDto>().ReverseMap();
    }
}