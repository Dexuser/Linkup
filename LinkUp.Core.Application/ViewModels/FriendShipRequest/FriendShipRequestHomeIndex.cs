using LinkUp.Core.Application.ViewModels.User;

namespace LinkUp.Core.Application.ViewModels.FriendShipRequest;
public class FriendShipRequestHomeIndex
{
    public required UserViewModel CurrentUser { get; set; }
    public List<FriendShipRequestViewModel> FriendShipRequests { get; set; } = [];
    public List<FriendShipRequestViewModel> FriendShipTargets{ get; set; } = [];
}   
