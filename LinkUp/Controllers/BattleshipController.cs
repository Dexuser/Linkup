using System.Security.Claims;
using AutoMapper;
using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.BattleshipGame;
using LinkUp.Core.Application.ViewModels.BattleshipGame.Game;
using LinkUp.Core.Application.ViewModels.BattleshipGame.Game.Enums;
using LinkUp.Core.Application.ViewModels.Ship;
using LinkUp.Core.Application.ViewModels.User;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LinkUp.Controllers;

public class BattleshipController : Controller
{
    private readonly IBattleshipGameService _battleshipGameService;
    private readonly IShipService _shipService;
    private readonly IAttackService _attackService;
    private readonly IMapper _mapper;

    public BattleshipController(IBattleshipGameService battleshipGameService, IMapper mapper, IShipService shipService, IAttackService attackService)
    {
        _battleshipGameService = battleshipGameService;
        _mapper = mapper;
        _shipService = shipService;
        _attackService = attackService;
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
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var missingShipsResult = await _shipService.GetMissingShips(userId, gameId);
        var gameResult = await _battleshipGameService.GetByIdAsync(gameId);
        
        if (gameResult.IsFailure)
        {
            this.SendValidationErrorMessages(gameResult);
            return View("Board",new BoardViewModel
            {
                GameId = gameId, 
                Type = BoardType.Position,
                IsMyTurn = false
            });
        }

        // Redirecciona al Preview de tu tablero si ya no tienes barcos y aun estamos en preparacion
        if (missingShipsResult.Value!.Count == 0 && gameResult.Value!.Status == GameStatus.SettingUp)
        {
            ViewBag.Message = "El otro jugador aun no ha terminado de posicionar todos sus barcos";
            return await RedirectToShipsPreviewBoard(gameId, userId);
        }

        if (gameResult.Value!.Status == GameStatus.InProgress)
        {
            return await RedirectToAttackBoard(gameId, userId);
        }
        
        return View("ShipSelection", new ShipSelectionViewModel()
        {
            GameId = gameId,
            MissingShips = missingShipsResult.Value!,
            
        });
    }

    public IActionResult GiveUpTheGame( int gameId, string userId, bool redirectToHome = false)
    {
        var viewmodel = new GiveUpViewModel
        {
            GameId = gameId,
            UserId = userId,
            RedirectToHome = redirectToHome
        };
        
        return View(viewmodel);
    }
    [HttpPost]
    public async Task<IActionResult> GiveUpTheGame( GiveUpViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var result = await _battleshipGameService.ThisUserGiveUp(model.GameId, model.UserId);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(model);
        }
        if (model.RedirectToHome)
        {
            RedirectToRoute(new { controller = "Battleship", action = "Index" });
        }
        return await RedirectToAttackBoard(model.GameId, model.UserId);
    }

    public IActionResult Results(int id)
    {
        return View();
    }

    
    public async Task<IActionResult> SetShipPosition(ShipSelectionViewModel viewModel)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var missingShipsResult = await _shipService.GetMissingShips(userId, viewModel.GameId);
        var gameResult = await _battleshipGameService.GetByIdAsync(viewModel.GameId);
        
        if (gameResult.IsFailure)
        {
            this.SendValidationErrorMessages(gameResult);
            return View("Board",new BoardViewModel
            {
                GameId = viewModel.GameId, 
                Type = BoardType.Position,
                IsMyTurn = false
            });
        }
        
        // Redirecciona al Preview de tu tablero si ya no tienes barcos y aun estamos en preparacion
        // La idea es que cuando se redireccione aqui cuando se eliga una direccion se mande para el preview
        if (missingShipsResult.Value!.Count == 0 && gameResult.Value!.Status == GameStatus.SettingUp)
        {
            return await RedirectToShipsPreviewBoard(viewModel.GameId, userId);
        }

        // Construir el tablero usando el método reutilizable
        var cells = await ConstructBoard(
            viewModel.GameId,
            userId,
            showShipsOfUser: true,
            makeCellSelectable: true,
            showAttacks: false
        );

        // Crear el BoardViewModel final
        var model = new BoardViewModel
        {
            GameId = viewModel.GameId,
            Type = BoardType.Position,
            IsMyTurn = (await _battleshipGameService.GetByIdAsync(viewModel.GameId)).Value!.CurrentTurnPlayerId == userId,
            SelectedShip = (ShipType)viewModel.SelectedShip,
            Cells = cells
        };

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
    public async Task<IActionResult> Attack(int gameid, int x, int y, int selectedShip)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var gameResult = await _battleshipGameService.GetByIdAsync(gameid);
        var newAttack = new AttackDto
        {
            GameId = gameid,
            AttackerId = userId,
            X = x,
            Y = y
            // isHit se encarga el servicio.
        };

        var createResult = await _attackService.AddAsync(newAttack);
        if (createResult.IsFailure)
        {
            this.SendValidationErrorMessages(createResult);
        }
        
        return await RedirectToAttackBoard(gameid, userId);
    }
    
        
    public async Task<IActionResult> ReturnToShipSelection(int gameId, int shipType)
    {
        return RedirectToAction("SetShipPositionFromParams", "Battleship", new { gameId = gameId, selectedShip = shipType });
    }
    
    // Esta sobrecarga toma parametros simples y prepara el modelo. Es para que el boton de ShipDirectionSelection pueda
    // retonar al Board
    public async Task<IActionResult> SetShipPositionFromParams(int gameId, int selectedShip)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        // Recuperar los barcos faltantes del servicio
        var missingShipsResult = await _shipService.GetMissingShips(userId, gameId);
        if (missingShipsResult.IsFailure)
        {
            this.SendValidationErrorMessages(missingShipsResult);
            return RedirectToAction("EnterTheGame", new { gameId });
        }

        // Construir el modelo que el método original necesita
        var viewModel = new ShipSelectionViewModel
        {
            GameId = gameId,
            MissingShips = missingShipsResult.Value!,
            SelectedShip = selectedShip
        };

        // Llamar al método original, que arma el tablero y devuelve View("Board")
        return await SetShipPosition(viewModel);
    }

    public async Task<List<List<CellViewModel>>> ConstructBoard(int gameId, string userId, bool showShipsOfUser = false, bool showAttacks = false,  bool makeCellSelectable=false)
    {
        
        // Paso 1: Traer datos de la base de datos
        var gameResult = await _battleshipGameService.GetByIdAsync(gameId);
        if (gameResult.IsFailure)
        {
            this.SendValidationErrorMessages(gameResult);
            //return View("ShipSelection", viewModel);
        }

        var game = gameResult.Value!;

        var cells = new List<List<CellViewModel>>();
        // Paso 2: Crear modelo base

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

            cells.Add(fila);
        }

        // Si es de ataque, no se debe de ver ningun barco y todas las celdas son seleccionables.
        if (showShipsOfUser)
        {
            // Paso 2: Marcar barcos ya posicionados
            var myShips = game.Ships.Where(s => s.PlayerId == userId);
            foreach (var ship in myShips)
            {
                foreach (var position in ship.Positions)
                {
                    var cell = cells[position.X][position.Y];
                    cell.State = CellStatus.Ship;
                    cell.CanBeSelected = false; // Ya ocupada
                }
            }
        }

        if (showAttacks)
        {
            var myAttacks = game.Attacks.Where(a => a.AttackerId == userId);
            foreach (var attack in myAttacks)
            {
                var cell = cells[attack.X][attack.Y];
                cell.State = attack.IsHit ? CellStatus.Impact : CellStatus.Miss;
                cell.CanBeSelected = false; // Ya ocupada
            }
        }

        if (makeCellSelectable)
        {
            // Paso 3: Hacer seleccionables las celdas vacías si hay barco seleccionado
            for (int x = 0; x < 12; x++)
            {
                for (int y = 0; y < 12; y++)
                {
                    var cell = cells[x][y];
                    if (cell.State == CellStatus.Empty)
                    {
                        cell.CanBeSelected = true;
                    }
                    else
                    {
                        // Si es Ship, Miss o Impact, no debe de ser posible atacar
                        cell.CanBeSelected = false;
                    }
                }
            }
        }

        return cells;
    }

    public async Task<IActionResult> RedirectToAttackPreviewBoard(int gameId, string userId, bool showOpponentBoard = false)
    {
        var userToShow = userId;
        if (showOpponentBoard)
        {
            var gameResult = await _battleshipGameService.GetByIdAsync(gameId);
            userToShow = gameResult.Value!.Player1Id ==  userId ? gameResult.Value!.Player2Id : gameResult.Value!.Player1Id;
        }
        var cellsPreview = await ConstructBoard(
            gameId,
            userToShow,
            showAttacks: true,
            makeCellSelectable: false,
            showShipsOfUser: false 
        );

        var modelPreview = new BoardViewModel
        {
            GameId = gameId,
            Type = BoardType.Results,
            IsMyTurn = (await _battleshipGameService.GetByIdAsync(gameId)).Value!.CurrentTurnPlayerId == userId,
            Cells = cellsPreview
        };

        return View("Board", modelPreview);
    }
    
    public async Task<IActionResult> RedirectToShipsPreviewBoard(int gameId, string userId)
    {
        var cellsPreview = await ConstructBoard(
            gameId,
            userId,
            showAttacks: false,
            makeCellSelectable: false,
            showShipsOfUser: true
        );

        var modelPreview = new BoardViewModel
        {
            GameId = gameId,
            Type = BoardType.Preview,
            IsMyTurn = (await _battleshipGameService.GetByIdAsync(gameId)).Value!.CurrentTurnPlayerId == userId,
            Cells = cellsPreview
        };

        return View("Board", modelPreview);
    }
    
    public  async Task<IActionResult> RedirectToAttackBoard(int gameId, string userId)
    {
        bool isMyTurn = (await _battleshipGameService.GetByIdAsync(gameId)).Value!.CurrentTurnPlayerId == userId;
        
        var gameEndedResult =  await _battleshipGameService.CheckIfGameEnded(gameId);
        if (gameEndedResult.IsFailure)
        {
            this.SendValidationErrorMessages(gameEndedResult);
            return await RedirectToAttackBoard(gameId, userId);
        }
        
        var gameEnded = gameEndedResult.Value!;
        if (gameEnded)
        {
            var gameResult = await _battleshipGameService.GetByIdAsync(gameId);
            if (gameResult.Value!.WinnerId == userId)
            {
                ViewBag.Message = $"Has ganado esta partida!";
            }
            else
            {
                ViewBag.Message = $"Has perdido esta partida!";
            }
            var model = new BoardViewModel
            {
                GameId = gameId,
                Type = BoardType.Attack,
                IsMyTurn = isMyTurn,
                Cells = await ConstructBoard(
                    gameId,
                    userId,
                    showAttacks: true,
                    makeCellSelectable: false,
                    showShipsOfUser: true
                )
            };

            return View("Board", model);
        }
        
        if (!isMyTurn)
        {
            ViewBag.Message = "El otro jugador esta atacando. Espera tu turno";
        }
        var cellsPreview = await ConstructBoard(
            gameId,
            userId,
            showAttacks: true,
            makeCellSelectable: isMyTurn,
            showShipsOfUser: false 
        );
        
        var modelPreview = new BoardViewModel
        {
            GameId = gameId,
            Type = BoardType.Attack,
            IsMyTurn = isMyTurn,
            Cells = cellsPreview
        };

        return View("Board", modelPreview);
    }
}