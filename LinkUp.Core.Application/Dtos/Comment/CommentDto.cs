using LinkUp.Core.Application.Dtos.User;

namespace LinkUp.Core.Application.Dtos.Comment;

public class CommentDto
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public required string Text { get; set; }
    
    public required string AuthorId { get; set; }
    public UserDto? Author { get; set; }
    public int? ParentCommentId { get; set; }
    
    public CommentDto? ParentComment { get; set; }
    public ICollection<CommentDto> Replies { get; set; } = [];
}