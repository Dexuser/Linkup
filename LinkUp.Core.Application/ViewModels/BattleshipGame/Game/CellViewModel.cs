using LinkUp.Core.Application.ViewModels.BattleshipGame.Game.Enums;

namespace LinkUp.Core.Application.ViewModels.BattleshipGame.Game;

public class CellViewModel
{
    public int X { get; set; }
    public int Y { get; set; }
    public CellStatus State { get; set; }
    public bool CanBeSelected { get; set; }
    public string UrlAction { get; set; } // URL a la que apunta el enlace
    public string CssClass => State switch
    {
        CellStatus.Empty => "cell-water", 
        CellStatus.Impact => "cell-hit",
        CellStatus.Ship => "cell-ship",
        CellStatus.Miss => "cell-miss",
        _ => "cell-empty"
    };
    public string Text { get; set; } // Texto a mostrar
}