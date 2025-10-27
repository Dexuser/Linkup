using AutoMapper;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Application.ViewModels.ShipPosition;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels;

public class ShipPositionDtoMappingProfile : Profile
{
    public ShipPositionDtoMappingProfile()
    {
        CreateMap<ShipPositionDto, ShipPositionViewModel>().ReverseMap();
    }
}