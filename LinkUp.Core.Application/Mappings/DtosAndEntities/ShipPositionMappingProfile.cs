using AutoMapper;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class ShipPositionMappingProfile : Profile
{
    public ShipPositionMappingProfile()
    {
        CreateMap<ShipPosition, ShipPositionDto>().ReverseMap();
    }
}