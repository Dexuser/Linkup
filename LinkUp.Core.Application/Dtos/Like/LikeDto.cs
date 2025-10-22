using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Domain.Entities;

namespace LinkUp.Core.Application.Dtos.Like;

public class LikeDto
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public required string UserId { get; set; }
    public required bool IsLiked { get; set; }
    
    public PostDto? Post { get; set; }
}