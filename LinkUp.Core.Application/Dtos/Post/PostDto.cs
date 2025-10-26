using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.User;

namespace LinkUp.Core.Application.Dtos.Post;

public class PostDto
{
    public required int Id { get; set; }
    public required string UserId { get; set; }
    public required string Text { get; set; }
    public string? ImagePath { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<CommentDto> Comments { get; set; } = []; // deberias de usar el Include
    public UserDto? User { get; set; }
    
    public bool? IsLikedByUserInThisSession { get; set; }
}