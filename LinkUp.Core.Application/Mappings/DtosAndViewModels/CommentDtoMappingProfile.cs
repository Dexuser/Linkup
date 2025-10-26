using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Viewmodels.Comment;
using LinkUp.Core.Domain.Entities;

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