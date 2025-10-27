using LinkUp.Core.Application.ViewModels.Comment;
using LinkUp.Core.Application.ViewModels.Post;

namespace LinkUp.Core.Application.ViewModels.HomeIndex;

public class HomeIndexViewModel
{
    public required List<PostViewModel> Posts { get; set; } = [];
    public required PostCreateViewModel PostCreateViewModel { get; set; }
    
    public required PostEditViewModel PostEditViewModel { get; set; }
    
    public required CommentCreateViewModel CommentCreateViewModel { get; set; }
    public required CommentEditViewModel CommentEditViewModel { get; set; }
    

}