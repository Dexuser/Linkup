using AutoMapper;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.ViewModels.BattleshipGame;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels;

public class BattleshipGameDtoMappingProfile : Profile
{
    public BattleshipGameDtoMappingProfile()
    {
        CreateMap<BattleshipGameDto, BattleshipGameViewModel>().ReverseMap();
    }
}