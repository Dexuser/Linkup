
namespace LinkUp.Core.Application.ViewModels.User;

public class UserWithCommonFriendCountViewModel
{
    public UserViewModel User { get; set; }
    public required int CommonFriendCount { get; set; }
}