using AutoMapper;
using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.ViewModels.Attack;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels;

public class AttackDtoMappingProfile : Profile
{
    public AttackDtoMappingProfile()
    {
        CreateMap<AttackDto, AttackViewModel>().ReverseMap();
    }
}