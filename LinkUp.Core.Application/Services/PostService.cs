using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;

namespace LinkUp.Core.Application.Services;

public class PostService : GenericServices<Post, PostDto>, IPostService 
{
    public PostService(IPostRepository repository, IMapper mapper) : base(repository, mapper)
    {
    }
}