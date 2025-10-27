namespace LinkUp.Core.Domain.Entities;

public class FriendShip
{
    public int Id { get; set; }
    public required string UserId1 { get; set; }
    public required string UserId2 { get; set; }
}