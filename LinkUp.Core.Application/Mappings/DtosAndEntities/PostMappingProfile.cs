using AutoMapper;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class PostMappingProfile : Profile
{
    public PostMappingProfile()
    {
        CreateMap<Post, PostDto>().ForMember(dest => dest.User, opt => opt.Ignore())
            .ReverseMap();
    }
}