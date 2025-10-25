using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.Viewmodels.FriendShipRequest;

public class FriendShipRequestViewModel
{
    public required int Id { get; set; }
    public required string RequestedByUserId { get; set; }
    public required string TargetUserId { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required FriendShipRequestStatus Status { get; set; }
}