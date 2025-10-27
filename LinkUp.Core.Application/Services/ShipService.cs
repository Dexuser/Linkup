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

    public ShipService(IShipRepository repository, IMapper mapper, IShipPositionService shipPositionService) : base(
        repository, mapper)
    {
        _shipRepository = repository;
        _shipPositionService = shipPositionService;
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
                return Result<ShipDto>.Fail("El barco choca con otro barco ya posicionado");
            }

            // 4. Si pasa todas las validaciones, agregar el barco
            var createResult = await base.AddAsync(dtoModel);
            if (createResult.IsFailure)
            {
                return Result<ShipDto>.Fail(createResult.GeneralError!);
            }

            // 5. Crear ShipPositions
            List<ShipPositionDto> shipPositions = new List<ShipPositionDto>();
            foreach (var Cell in cells)
            {
                shipPositions.Add(new ShipPositionDto()
                {
                    Id = 0,
                    ShipId = createResult.Value!.Id,
                    X = Cell[0],
                    Y = Cell[1],
                    IsHit = false
                });
            }

            var createShipPositionResult = await _shipPositionService.AddRangeAsync(shipPositions);

            if (createShipPositionResult.IsFailure)
            {
                return Result<ShipDto>.Fail(createShipPositionResult.GeneralError!);
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
            var requiredShips = new List<ShipType>
            {
                ShipType.PatrolBoat,
                ShipType.Destroyer,
                ShipType.Destroyer,
                ShipType.Battleship,
                ShipType.Carrier
            };

            var placedShips = await _shipRepository
                .GetAllQueryable()
                .Where(s => s.GameId == gameId && s.PlayerId == userId)
                .Select(s => s.Type)
                .ToListAsync();

            // Filtro manual para evitar el problema del valor duplicado
            var missingShips = requiredShips
                .Where(required => !placedShips.Any(placed => placed == required))
                .ToList();

            return Result<List<ShipType>>.Ok(missingShips);
        }
        catch (Exception e)
        {
            return Result<List<ShipType>>.Fail(e.Message);
        }
    }
}