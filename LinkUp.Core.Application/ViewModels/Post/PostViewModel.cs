using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Viewmodels.Comment;
using LinkUp.Core.Application.Viewmodels.User;

namespace LinkUp.Core.Application.Viewmodels.Post;

public class PostViewModel
{
    public required int Id { get; set; }
    public required string UserId { get; set; }
    public required string Text { get; set; }
    public string? ImagePath { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<CommentViewModel> Comments { get; set; } = []; // deberias de usar el Include
    public UserViewModel? User { get; set; }
    public bool? IsLikedByUserInThisSession { get; set; }
}