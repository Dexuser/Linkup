namespace LinkUp.Core.Application.ViewModels.BattleshipGame.Game.Enums;

public enum CellStatus
{
    Empty, // No hay nada
    Impact, // se ataco a unbarco
    Ship, // un bloque que esta pisando un barco y no ha sido atacado
    SunkShip, // un bloque que esta pisando un barco y ha sido atacado
    Invalid, // un bloque que ya no se puede tocar
}