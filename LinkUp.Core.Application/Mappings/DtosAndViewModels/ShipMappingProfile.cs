using AutoMapper;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.ViewModels.Ship;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels;

public class ShipMappingProfile : Profile
{
    public ShipMappingProfile()
    {
        CreateMap<ShipDto, ShipViewModel>().ReverseMap();
    }
}