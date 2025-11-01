
using AutoMapper;
using LinkUp.Core.Application.Dtos.BattleshipGame;
using LinkUp.Core.Application.Dtos.ShipPosition;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Application.Services;

public class BattleshipGameService : GenericServices<BattleshipGame, BattleshipGameDto>, IBattleshipGameService
{
    private readonly IBattleshipGameRepository _battleshipGameRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IMapper _mapper;
    public BattleshipGameService(IBattleshipGameRepository repository, IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp, IFriendShipRepository friendShipRepository) : base(repository, mapper)
    {
        _battleshipGameRepository = repository;
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _friendShipRepository = friendShipRepository;
    }

    public override async Task<Result<BattleshipGameDto>> GetByIdAsync(int id)
    {
        var game = await _battleshipGameRepository.GetAllQueryable().AsNoTracking()
            .Include(g => g.Ships)
                .ThenInclude(s => s.Positions)
            .Include(g => g.Attacks)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (game == null)
            return Result<BattleshipGameDto>.Fail("No se encontro esta partida");
        var battleshipGameDto = _mapper.Map<BattleshipGameDto>(game);
        
        return Result<BattleshipGameDto>.Ok(battleshipGameDto);
    }

    public async Task<Result<List<BattleshipGameDto>>> GetAllBattleshipGamesOfUser(string userId)
    {
        try
        {
            var games = await _battleshipGameRepository.GetAllQueryable().AsNoTracking()
                .Where(g => g.Player1Id == userId || g.Player2Id == userId)
                .ToListAsync();

            var playersIds = games.Select(g => g.Player1Id == userId ? g.Player2Id : g.Player1Id)
                .Distinct()
                .ToList();

            playersIds.Add(userId);
            var playersResult = await _accountServiceForWebApp.GetUsersByIds(playersIds);
            var players = playersResult.Value!.ToDictionary(p => p.Id, p => p);
            var gamesDtos = _mapper.Map<List<BattleshipGameDto>>(games);

            foreach (var game in gamesDtos)
            {
                game.Player1 = players.GetValueOrDefault(game.Player1Id);
                game.Player2 = players.GetValueOrDefault(game.Player2Id);
            }

            return Result<List<BattleshipGameDto>>.Ok(gamesDtos);
        }
        catch (Exception ex)
        {
            return Result<List<BattleshipGameDto>>.Fail(ex.Message);
        }
    }

    // Teniendo en cuenta que los objetos son tipos de referencia, los DTO que pasen por este metodo
    // seran modificados. Por eso no vuelvo a pedir un listasdo a la DB
    private async Task CheckForAbandonedGames(List<BattleshipGameDto> gamess)
    {
        
        foreach (var game in gamess)
        {
            if (DateTime.Now - game.LastMoveDate <= TimeSpan.FromHours(48)) continue;
            game.EndDate = DateTime.Now;
            game.WinnerId = game.CurrentTurnPlayerId == game.Player1Id ? game.Player2Id : game.Player1Id;
            game.Status = GameStatus.Finished;
            await UpdateAsync(game.Id, game);
        }
    }
    
    public async Task<Result<List<UserDto>>> GetAllTheUsersAvailableForAGame(string userId, string? userNameFilter = null)
    {
        try
        {
            // IDs de usuarios que ya son amigos
            var friendsIds = await _friendShipRepository.GetAllTheFriendsIds(userId);

            // IDs de usuarios con juegos activos
            var usersIdsOfOpponentsInActiveGames = await _battleshipGameRepository.GetAllQueryable()
                .AsNoTracking()
                .Where(g => g.Status == GameStatus.InProgress || g.Status == GameStatus.SettingUp)
                .Where(g => g.Player1Id == userId || g.Player2Id == userId)
                .Select(r => r.Player1Id == userId ? r.Player2Id : r.Player1Id)
                .ToListAsync();
        
            var potentialOpponents = friendsIds.Except(usersIdsOfOpponentsInActiveGames).ToList();
            var usersResult = await _accountServiceForWebApp.GetUsersByIds(potentialOpponents);
            var users = usersResult.Value!;
        
            if (!string.IsNullOrWhiteSpace(userNameFilter))
            {
                users = users.Where(u=> u.UserName.Contains(userNameFilter)).ToList();
            }
        
            return Result<List<UserDto>>.Ok(users);

        }
        catch (Exception e)
        {
            return Result<List<UserDto>>.Fail(e.Message);
        }
    }

    public async Task<Result> SetStatus(int gameId, GameStatus status)
    {
        if (await _battleshipGameRepository.SetStatus(gameId, status))
        {
            return Result.Ok();
        }
        return Result.Fail("No se encontro esta partida");
    }

    public async Task<Result> ChangeTurn(int gameId)
    {
        if (await _battleshipGameRepository.ChangeTurn(gameId))
        {
         return Result.Ok();   
        }
        return Result.Fail("No se encontro esta partida");
    }

    
    public async Task<Result<bool>> CheckIfGameEnded(int gameId)
    {
        var gameResult = await GetByIdAsync(gameId);
        if (gameResult.IsFailure)
            return Result<bool>.Fail(gameResult.GeneralError!);

        var game = gameResult.Value!;
    
        // Si ya terminó, retornar true
        if (game.Status == GameStatus.Finished)
            return Result<bool>.Ok(true);

        var player1Ships = game.Ships.Where(s => s.PlayerId == game.Player1Id);
        var player2Ships = game.Ships.Where(s => s.PlayerId == game.Player2Id);

        // Verificar si algún jugador perdió todos sus barcos
        var player1Lost = player1Ships.All(s => s.IsSunk);
        var player2Lost = player2Ships.All(s => s.IsSunk);

        if (player1Lost || player2Lost)
        {
            game.Status = GameStatus.Finished;
            game.WinnerId = player1Lost ? game.Player2Id : game.Player1Id;
            game.EndDate = DateTime.Now;
        
            await UpdateAsync(gameId,game);  // Guardar cambios
            return Result<bool>.Ok(true);
        }

        return Result<bool>.Ok(false);
    }

    public async Task<Result> UpdateLastMove(int gameId)
    {
        try
        {
            await _battleshipGameRepository.UpdateLastMove(gameId);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result> ThisUserGiveUp(int gameId, string userId)
    {
        var gameResult = await GetByIdAsync(gameId);
        if (gameResult.IsFailure)
        {
            return Result.Fail(gameResult.GeneralError!);
        }
        var game = gameResult.Value!;
        game.WinnerId = game.Player1Id ==  userId ? game.Player2Id : game.Player1Id;
        game.EndDate = DateTime.Now;
        game.Status = GameStatus.Finished;
        var updateResult = await UpdateAsync(gameId, game);
        if (updateResult.IsFailure)
        {
            return Result.Fail(updateResult.GeneralError!);
        }
        return Result.Ok();
    }
    
    public async Task<Result<BattleshipGameSummaryDto>> GetSummaryOfThisUser(string userId)
    {
        var query = _battleshipGameRepository.GetAllQueryable();
    
        var summary = new BattleshipGameSummaryDto
        {
            GamesCount = await query
                .CountAsync(g => g.Player1Id == userId || g.Player2Id == userId),
            
            GamesWinCount = await query
                .CountAsync(g => g.WinnerId == userId),
            
            GamesLoseCount = await query
                .CountAsync(g => (g.Player1Id == userId || g.Player2Id == userId) && 
                                 g.WinnerId != null && 
                                 g.WinnerId != userId &&
                                 g.Status == GameStatus.Finished)
        };

        return Result<BattleshipGameSummaryDto>.Ok(summary);
    }
}