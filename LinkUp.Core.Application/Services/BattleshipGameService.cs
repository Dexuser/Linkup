
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
}