using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Post;

namespace LinkUp.Core.Application.Interfaces;

public interface IPostService : IGenericService<PostDto>
{
    Task<Result<List<PostDto>>> GetAllPostsOfThisUser(string userId);
    Task<Result<List<PostDto>>> GetAllFriendsPosts(string userId);

}