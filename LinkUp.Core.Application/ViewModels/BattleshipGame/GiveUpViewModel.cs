namespace LinkUp.Core.Application.ViewModels.BattleshipGame;

public class GiveUpViewModel
{
    public required int GameId { get; set; }
    public required string UserId { get; set; }
    public required bool RedirectToHome { get; set; }
}