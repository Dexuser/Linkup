using AutoMapper;
using LinkUp.Core.Application.Dtos.Ship;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Application.Services;

public class ShipService : GenericServices<Ship, ShipDto>, IShipService
{
    private readonly IShipRepository _shipRepository;
    private readonly IShipPositionService _shipPositionService;
    private readonly IBattleshipGameService _battleshipGameService;
    private readonly IMapper _mapper;

    public ShipService(IShipRepository repository, IMapper mapper, IShipPositionService shipPositionService, IBattleshipGameService battleshipGameService) : base(
        repository, mapper)
    {
        _shipRepository = repository;
        _mapper = mapper;
        _shipPositionService = shipPositionService;
        _battleshipGameService = battleshipGameService;
    }

    public override async Task<Result<ShipDto>> AddAsync(ShipDto dtoModel)
    {
        try
        {
            // 1. Calcular todas las celdas que ocupará el barco
            List<int[]> cells = new List<int[]>();
            for (int i = 0; i < dtoModel.Size; i++)
            {
                switch (dtoModel.Direction)
                {
                    case Direction.Up:
                        cells.Add([dtoModel.StartX - i, dtoModel.StartY]);
                        break;
                    case Direction.Down:
                        cells.Add([dtoModel.StartX + i, dtoModel.StartY]);
                        break;
                    case Direction.Left:
                        cells.Add([dtoModel.StartX, dtoModel.StartY - i]);
                        break;
                    case Direction.Right:
                        cells.Add([dtoModel.StartX, dtoModel.StartY + i]);
                        break;
                }
            }

            // 2. Validar que no desborde el tablero
            if (cells.Any(c => c[0] < 0 || c[0] > 11 || c[1] < 0 || c[1] > 11))
            {
                return Result<ShipDto>.Fail("Esta dirección hace que el barco desborde el tablero");
            }

            // 3. Validar que no choque con barcos existentes del mismo jugador
            var existingShipsPositions = await _shipRepository.GetAllQueryable()
                .Where(s => s.GameId == dtoModel.GameId && s.PlayerId == dtoModel.PlayerId)
                .SelectMany(s => s.Positions)
                .Select(p => new { p.X, p.Y })
                .ToListAsync();

            // Verificar si alguna celda nueva coincide con posiciones existentes
            var collision = cells.Any(newCell =>
                existingShipsPositions.Any(existing =>
                    existing.X == newCell[0] && existing.Y == newCell[1]));

            if (collision)
            {
                return Result<ShipDto>.Fail(
                    "Debe cambiar la celda seleccionada o la dirección," +
                    " ya que con la combinación actual el barco quedaría " +
                    "posicionado encima de otro barco");
            }

            // 4. Si pasa todas las validaciones, agregar el barco
            var createResult = await base.AddAsync(dtoModel);
            if (createResult.IsFailure)
            {
                return Result<ShipDto>.Fail(createResult.GeneralError!);
            }
            await _battleshipGameService.UpdateLastMove(dtoModel.GameId);

            // 5. Crear ShipPositions
            List<ShipPositionDto> shipPositions = new List<ShipPositionDto>();
            foreach (var cell in cells)
            {
                shipPositions.Add(new ShipPositionDto()
                {
                    Id = 0,
                    ShipId = createResult.Value!.Id,
                    X = cell[0],
                    Y = cell[1],
                    IsHit = false
                });
            }

            var createShipPositionResult = await _shipPositionService.AddRangeAsync(shipPositions);

            if (createShipPositionResult.IsFailure)
            {
                return Result<ShipDto>.Fail(createShipPositionResult.GeneralError!);
            }

            // Si ya los jugadores pusieron todos sus barcos, comenzar la fase de ataque
            int gameShipCount = _shipRepository.GetAllQueryable().Count(s => s.GameId == dtoModel.GameId);
            if (gameShipCount == 10)
            {
                await _battleshipGameService.SetStatus(createResult.Value!.GameId, GameStatus.InProgress);
            }
            
            var returnShipDto = createResult!.Value;
            returnShipDto.Positions = createShipPositionResult.Value!;

            return Result<ShipDto>.Ok(returnShipDto);
        }
        catch (Exception ex)
        {
            return Result<ShipDto>.Fail(ex.Message);
        }
    }

    public async Task<Result<List<ShipType>>> GetMissingShips(string userId, int gameId)
    {
        try
        {
            // Cuántos barcos de cada tipo requiere el juego
            var requiredShips = new Dictionary<ShipType, int>
            {
                { ShipType.Carrier, 1 },
                { ShipType.Battleship, 1 },
                { ShipType.Destroyer, 2 },
                { ShipType.PatrolBoat, 1 }
            };

            // Obtener todos los barcos colocados por el jugador en este juego
            var placedShips = await _shipRepository
                .GetAllQueryable()
                .Where(s => s.GameId == gameId && s.PlayerId == userId)
                .Select(s => s.Type)
                .ToListAsync();

            // Contar cuántos barcos de cada tipo ya están colocados
            var placedCounts = placedShips
                .GroupBy(s => s)
                .ToDictionary(g => g.Key, g => g.Count());

            // Crear la lista de barcos faltantes (repetidos según la cantidad que falte)
            var missingShips = new List<ShipType>();

            foreach (var kvp in requiredShips)
            {
                var type = kvp.Key;
                var requiredCount = kvp.Value;
                var placedCount = placedCounts.GetValueOrDefault(type, 0);
                var remaining = requiredCount - placedCount;

                // Agregar el tipo N veces, donde N = remaining
                for (int i = 0; i < remaining; i++)
                {
                    missingShips.Add(type);
                }
            }

            return Result<List<ShipType>>.Ok(missingShips);
        }
        catch (Exception e)
        {
            return Result<List<ShipType>>.Fail(e.Message);
        }
    }
    
    public async Task<Result> CheckIfShipSunk(int shipId)
    {
        var ship = await _shipRepository.GetAllQueryable()
            .Include(s => s.Positions)
            .FirstOrDefaultAsync(s => s.Id == shipId);

        if (ship == null)
        {
            return Result<ShipDto>.Fail("No se encontro un barco con ese ID");
        }

        // Verificar si TODAS las posiciones del barco han sido impactadas
        var allPositionsHit = ship.Positions.All(p => p.IsHit);

        if (allPositionsHit)
        {
            var shipDto = _mapper.Map<ShipDto>(ship);
            
            shipDto.IsSunk = true;
            var updateResult = await UpdateAsync(shipDto.Id,shipDto);
            if (updateResult.IsFailure)
            {
                return  Result<ShipDto>.Fail(updateResult.GeneralError!);
            }
        
            var gameEndedResult =  await _battleshipGameService.CheckIfGameEnded(ship.GameId);
            if (gameEndedResult.IsFailure)
            {
                return  Result<ShipDto>.Fail(gameEndedResult.GeneralError!);
            }
        }
        
        return Result.Ok();
    }

}