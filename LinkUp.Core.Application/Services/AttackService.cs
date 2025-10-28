using AutoMapper;
using LinkUp.Core.Application.Dtos.Attack;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Application.Services;

public class AttackService : GenericServices<Attack, AttackDto>, IAttackService
{
    private readonly IBattleshipGameService _battleshipGameService;
    private readonly IAttackRepository _attackRepository;
    private readonly IShipRepository _shipRepository;
    private readonly IShipService _shipService;
    private readonly IShipPositionService _shipPositionService;
    private readonly IMapper _mapper;
    public AttackService(IAttackRepository repository, IMapper mapper, IAttackRepository attackRepository, IShipRepository shipRepository, IShipService shipService, IBattleshipGameService battleshipGameService, IShipPositionService shipPositionService) : base(repository, mapper)
    {
        _mapper = mapper;
        _attackRepository = attackRepository;
        _shipRepository = shipRepository;
        _shipService = shipService;
        _battleshipGameService = battleshipGameService;
        _shipPositionService = shipPositionService;
    }

    public override async Task<Result<AttackDto>> AddAsync(AttackDto dtoModel)
    {
        try
        {
            // Validar que la celda no haya sido atacada antes
            var alreadyAttacked = await _attackRepository.GetAllQueryable()
                .AnyAsync(a =>
                    a.GameId == dtoModel.GameId && a.AttackerId == dtoModel.AttackerId && a.X == dtoModel.X &&
                    a.Y == dtoModel.Y);

            if (alreadyAttacked)
            {
                return Result<AttackDto>.Fail("Esta celda ya fue atacada");
            }

            // Verificar si hay un barco enemigo en esa posición
            var hitShip = await _shipRepository.GetAllQueryable()
                .Where(s => s.GameId == dtoModel.GameId && s.PlayerId != dtoModel.AttackerId) // Barcos del oponente
                .SelectMany(s => s.Positions)
                .FirstOrDefaultAsync(p => p.X == dtoModel.X && p.Y == dtoModel.Y);

            var isHit = hitShip != null;
            dtoModel.IsHit = isHit;
            dtoModel.AttackTime = DateTime.Now;
            // Creamos el ataque
            var createResult = await base.AddAsync(dtoModel);
            await _battleshipGameService.UpdateLastMove(dtoModel.GameId);
            

            if (createResult.IsFailure)
            {
                return Result<AttackDto>.Fail(createResult.GeneralError!);
            }

            if (isHit)
            {
                hitShip.IsHit = true;
                var shipPositionDto = _mapper.Map<ShipPositionDto>(hitShip);
                var updateResult = await _shipPositionService.UpdateAsync(shipPositionDto.Id,shipPositionDto);
                if (updateResult.IsFailure)
                {
                    return Result<AttackDto>.Fail(createResult.GeneralError!);
                }
                
                await _shipService.CheckIfShipSunk(hitShip.ShipId);
            }

            // Cambiar turno
            var changeResult = await _battleshipGameService.ChangeTurn(createResult.Value!.GameId);
            if (changeResult.IsFailure)
            {
                return Result<AttackDto>.Fail(changeResult.GeneralError!);
            }

            return Result<AttackDto>.Ok(createResult.Value!);
        }
        catch (Exception ex)
        {
            return Result<AttackDto>.Fail(ex.Message);
        }
    }
}