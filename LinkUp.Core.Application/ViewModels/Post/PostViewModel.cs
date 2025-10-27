using LinkUp.Core.Application.ViewModels.Comment;
using LinkUp.Core.Application.ViewModels.User;

namespace LinkUp.Core.Application.ViewModels.Post;

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