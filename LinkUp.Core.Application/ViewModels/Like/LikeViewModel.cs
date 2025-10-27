using LinkUp.Core.Application.ViewModels.Post;

namespace LinkUp.Core.Application.ViewModels.Like;

public class LikeViewModel
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public required string UserId { get; set; }
    public required bool IsLiked { get; set; }
    
    public PostViewModel? Post { get; set; }
}