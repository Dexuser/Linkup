using AutoMapper;
using LinkUp.Core.Application.Dtos.FriendShipRequest;
using LinkUp.Core.Application.ViewModels.FriendShipRequest;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels;

public class FriendShipRequestDtoMappingProfile : Profile
{
    public FriendShipRequestDtoMappingProfile()
    {
        CreateMap<FriendShipRequestDto, FriendShipRequestViewModel>().ReverseMap();
    }
}