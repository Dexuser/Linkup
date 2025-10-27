using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Mappings.DtosAndEntities;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        CreateMap<Comment, CommentDto>().ReverseMap();
    }
}