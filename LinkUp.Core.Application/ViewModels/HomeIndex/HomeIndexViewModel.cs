using LinkUp.Core.Application.Viewmodels.Comment;
using LinkUp.Core.Application.Viewmodels.Post;

namespace LinkUp.Core.Application.Viewmodels.HomeIndex;

public class HomeIndexViewModel
{
    public required List<PostViewModel> Posts { get; set; } = [];
    public required PostCreateViewModel PostCreateViewModel { get; set; }
    public required CommentCreateViewModel CommentCreateViewModel { get; set; }
}