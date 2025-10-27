namespace LinkUp.Core.Application.Dtos.FriendShip;

public class FriendShipDto
{
    public int Id { get; set; }
    public required string UserId1 { get; set; }
    public required string UserId2 { get; set; }
}