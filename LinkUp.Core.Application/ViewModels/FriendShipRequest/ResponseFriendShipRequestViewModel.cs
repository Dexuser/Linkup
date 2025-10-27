namespace LinkUp.Core.Application.ViewModels.FriendShipRequest;

public class ResponseFriendShipRequestViewModel
{ 
        public required int FriendShipRequestId { get; set; } 
        public required string UserName { get; set; }
        public required bool Accepted { get; set; }
}