using LinkUp.Core.Application.ViewModels.Comment;
using LinkUp.Core.Application.ViewModels.Post;
using LinkUp.Core.Application.ViewModels.User;

namespace LinkUp.Core.Application.ViewModels.Friends;

public class FriendsIndexViewModel
{
    public required UserViewModel CurrentUser { get; set; } 
    public required List<PostViewModel> Posts { get; set; } = [];
    
    public required List<UserViewModel> Friends { get; set; } = [];
    public required CommentCreateViewModel CommentCreateViewModel { get; set; }
    public required CommentEditViewModel CommentEditViewModel { get; set; }
}