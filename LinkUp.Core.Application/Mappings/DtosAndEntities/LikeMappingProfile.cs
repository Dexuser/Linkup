using AutoMapper;
using LinkUp.Core.Application.Dtos.Like;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class LikeMappingProfile : Profile
{
    public LikeMappingProfile()
    {
        CreateMap<Like, LikeDto>().ReverseMap();
    }
}