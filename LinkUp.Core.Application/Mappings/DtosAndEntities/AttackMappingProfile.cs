using AutoMapper;
using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class AttackMappingProfile : Profile
{
    public AttackMappingProfile()
    {
        CreateMap<Attack, AttackDto>().ReverseMap();
    }
}