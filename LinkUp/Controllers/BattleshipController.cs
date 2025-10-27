using System.Security.Claims;
using AutoMapper;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.BattleshipGame;
using LinkUp.Core.Application.ViewModels.BattleshipGame.Game;
using LinkUp.Core.Application.ViewModels.BattleshipGame.Game.Enums;
using LinkUp.Core.Application.ViewModels.Ship;
using LinkUp.Core.Application.ViewModels.User;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class BattleshipController : Controller
{
    private readonly IBattleshipGameService _battleshipGameService;
    private readonly IShipService _shipService;
    private readonly IMapper _mapper;

    public BattleshipController(IBattleshipGameService battleshipGameService, IMapper mapper, IShipService shipService)
    {
        _battleshipGameService = battleshipGameService;
        _mapper = mapper;
        _shipService = shipService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var viewmodel = new BattleshipGameHomeIndex();
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var requestResult = await _battleshipGameService.GetAllBattleshipGamesOfUser(userId);

        if (requestResult.IsFailure)
        {
            this.SendValidationErrorMessages(requestResult);
            return View(viewmodel);
        }

        var requests = requestResult.Value;
        var activeGames = requests.Where(g => g.Status == GameStatus.InProgress || g.Status == GameStatus.SettingUp)
            .ToList();
        var toOthers = requests.Where(g => g.Status == GameStatus.Finished || g.Status == GameStatus.Abandoned)
            .ToList();
        viewmodel.ActiveGames = _mapper.Map<List<BattleshipGameViewModel>>(activeGames);
        viewmodel.FinalizedGames = _mapper.Map<List<BattleshipGameViewModel>>(toOthers);

        return View(viewmodel);
    }

    public async Task<IActionResult> Create(string? usernameFilter = null)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var userDtoResult = await _battleshipGameService.GetAllTheUsersAvailableForAGame(userId, usernameFilter);
        if (userDtoResult.IsFailure)
        {
            this.SendValidationErrorMessages(userDtoResult);
            return View(new List<UserViewModel>());
        }

        var viewmodel = _mapper.Map<List<UserViewModel>>(userDtoResult.Value!);
        return View(viewmodel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame(string selectedUserId)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var newGame = new BattleshipGameDto
        {
            Player1Id = userId,
            Player2Id = selectedUserId,
            CurrentTurnPlayerId = userId,
            StartDate = DateTime.Now
        };
        var createResult = await _battleshipGameService.AddAsync(newGame);
        if (createResult.IsFailure)
        {
            this.SendValidationErrorMessages(createResult);
            return View("Create", new List<UserViewModel>());
        }

        return RedirectToRoute(new { controller = "Battleship", action = "Index" });
    }


    public async Task<IActionResult> EnterTheGame(int gameId)
    {
        // TODO validar que todos los barcos esten puestos para redireccionar al board de atatque
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var missingShips = await _shipService.GetMissingShips(userId, gameId);
        return View("ShipSelection", new ShipSelectionViewModel()
        {
            GameId = gameId,
            MissingShips = missingShips.Value!,
            
        });
    }

    public IActionResult GiveUpTheGame()
    {
        return View();
    }

    public IActionResult Results(int id)
    {
        return View();
    }

    // vista de seleccion de barco -> board
    public async Task<IActionResult> SetShipPosition(ShipSelectionViewModel viewModel)
    {

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        // Paso 1: Traer datos de la base de datos
        var gameResult = await _battleshipGameService.GetByIdAsync(viewModel.GameId);
        if (gameResult.IsFailure)
        {
            this.SendValidationErrorMessages(gameResult);
            return View("ShipSelection", viewModel);
        }

        var game = gameResult.Value!;


        // Paso 2: Crear modelo base
        var model = new BoardViewModel()
        {
            GameId = viewModel.GameId,
            Type = BoardType.Position,
            IsMyTurn = game.CurrentTurnPlayerId == userId,
            SelectedShip = (ShipType)viewModel.SelectedShip,
            Cells = new List<List<CellViewModel>>()
        };

        // Paso 3: Inicializar matriz 12x12 con celdas vacías
        for (int x = 0; x < 12; x++)
        {
            var fila = new List<CellViewModel>();
            for (int y = 0; y < 12; y++)
            {
                fila.Add(new CellViewModel
                {
                    X = x,
                    Y = y,
                    State = CellStatus.Empty,
                    CanBeSelected = false // Temporal
                });
            }

            model.Cells.Add(fila);
        }

        // Paso 2: Marcar barcos ya posicionados
        var myShips = game.Ships.Where(s => s.PlayerId == userId);
        foreach (var ship in myShips)
        {
            foreach (var position in ship.Positions)
            {
                var cell = model.Cells[position.X][position.Y];
                cell.State = ship.IsSunk ? CellStatus.SunkShip : CellStatus.Ship;
                cell.CanBeSelected = false; // Ya ocupada
            }
        }

        // Paso 3: Hacer seleccionables las celdas vacías si hay barco seleccionado
        if (viewModel.SelectedShip != null)
        {
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 12; y++)
                {
                    var cell = model.Cells[x][y];
                    if (cell.State == CellStatus.Empty)
                    {
                        cell.CanBeSelected = true;
                    }
                }
            }
        }

        return View("Board", model);
    }

    public async Task<IActionResult> SetShipPositionPost(int gameid, int x, int y, int selectedShip)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var gameResult = await _battleshipGameService.GetByIdAsync(gameid);
        var newShipViewModel = new ShipViewModel()
        {
            Id = 0,
            GameId = gameid,
            PlayerId = userId,
            Type = (ShipType) selectedShip,
            Size = selectedShip,
            StartX = x,
            StartY = y,
            IsSunk = false,
            // Solamente nos falta la direccion. Lo llenara ShipDirectionSelection
        };
        return View("ShipDirectionSelection", newShipViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateShip(ShipViewModel viewModel)
    {
        var shipDto = _mapper.Map<ShipDto>(viewModel);
        var createResult = await _shipService.AddAsync(shipDto);
        if (createResult.IsFailure)
        {
            this.SendValidationErrorMessages(createResult);
            return View("ShipDirectionSelection", viewModel);
        }
        return RedirectToRoute(new { controller = "Battleship", action = "EnterTheGame",  gameid = viewModel.GameId ,  });
    }
}