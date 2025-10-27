namespace LinkUp.Core.Application.Dtos.User;

public class UserWithCommonFriendCountDto
{
    public UserDto User { get; set; }
    public required int CommonFriendCount { get; set; }
}