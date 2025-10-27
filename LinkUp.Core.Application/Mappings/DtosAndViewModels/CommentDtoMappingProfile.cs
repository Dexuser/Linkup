using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.ViewModels.Comment;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels;

public class CommentDtoMappingProfile : Profile
{
    public CommentDtoMappingProfile()
    {
        CreateMap<CommentDto, CommentViewModel>().ReverseMap();
        CreateMap<CommentEditViewModel, CommentDto>().ReverseMap();
        CreateMap<CommentCreateViewModel, CommentDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
    }
}