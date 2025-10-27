using AutoMapper;
using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class ShipMappingProfile : Profile
{
    public ShipMappingProfile()
    {
        CreateMap<Ship, ShipDto>().ReverseMap();
    }
}