using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;

namespace LinkUp.Core.Application.Services;

public class CommentService : GenericServices<Comment, CommentDto>, ICommentService
{
    public CommentService(ICommentRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}