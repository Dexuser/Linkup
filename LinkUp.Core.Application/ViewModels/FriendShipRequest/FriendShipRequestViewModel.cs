using LinkUp.Core.Application.ViewModels.User;
using LinkUp.Core.Domain.Entities.Common;

namespace LinkUp.Core.Application.ViewModels.FriendShipRequest;

public class FriendShipRequestViewModel
{
    public required int Id { get; set; }
    public required string RequestedByUserId { get; set; }
    public required string TargetUserId { get; set; }
    public required int CommonFriendsCount { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required FriendShipRequestStatus Status { get; set; }
        
    public string StatusText
    {
        get
        {
            return Status switch
            {
                FriendShipRequestStatus.Accepted => "Aceptada",
                FriendShipRequestStatus.Rejected => "Rechazada",
                FriendShipRequestStatus.Pending => "Pendiente",
                _ => "Desconocido"
            };
        }
    }
    public UserViewModel? RequestedByUser { get; set; }
    public UserViewModel? TargetUser { get; set; }
}