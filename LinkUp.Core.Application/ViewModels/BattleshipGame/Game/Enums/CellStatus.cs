namespace LinkUp.Core.Application.ViewModels.BattleshipGame.Game.Enums;

public enum CellStatus
{
    Empty, // No hay nada, normal, azul
    Impact, // se ataco a un barco rojo
    Ship, // un bloque que esta pisando un barco y no ha sido atacado gris
    Miss, // se ataco, no se dio a ningun barco verde
}