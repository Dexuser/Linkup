using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Viewmodels.User;

namespace LinkUp.Core.Application.Viewmodels.Comment;

public class CommentViewModel
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public required string Text { get; set; }
    
    public required string AuthorId { get; set; }
    public UserViewModel? Author { get; set; }
    public int? ParentCommentId { get; set; }
    
    public CommentDto? ParentComment { get; set; }
    public ICollection<CommentDto> Replies { get; set; } = [];
}