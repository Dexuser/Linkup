using AutoMapper;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Application.ViewModels.Post;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels;

public class PostDtoMappingProfile : Profile
{
    public PostDtoMappingProfile()
    {
        CreateMap<PostCreateViewModel, PostDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Comments, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        CreateMap<PostDto, PostViewModel>();
    }
}