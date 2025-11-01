using AutoMapper;
using LinkUp.Core.Application.Dtos.FriendShip;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace LinkUp.Core.Application.Services;

public class FriendShipService : GenericServices<FriendShip, FriendShipDto>, IFriendShipService
{
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IFriendShipRequestRepository _friendShipRequestRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IMapper _mapper;

    public FriendShipService(IFriendShipRepository repository, IMapper mapper,
        IAccountServiceForWebApp accountServiceForWebApp,
        IFriendShipRequestRepository friendShipRequestRepository) : base(repository, mapper)
    {
        _friendShipRepository = repository;
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _friendShipRequestRepository = friendShipRequestRepository;
    }

    public override async Task<Result<FriendShipDto>> AddAsync(FriendShipDto dtoModel)
    {
        bool thereIsADuplicate  = await _friendShipRepository.GetAllQueryable()
            .AsNoTracking()
            .AnyAsync(fr => 
                fr.UserId1 == dtoModel.UserId1 && fr.UserId2 == dtoModel.UserId2
                ||
                fr.UserId1 ==  dtoModel.UserId2 && fr.UserId2 == dtoModel.UserId1
                );

        if (thereIsADuplicate)
        {
            return Result<FriendShipDto>.Fail("Ya existe una amistad entre estos usuarios");
        }
        return await base.AddAsync(dtoModel);
    }
    
    public async Task<int> GetCommonFriendsCountAsync(string userId1, string userId2)
    {
        var friendsUser1 = await _friendShipRepository.GetAllTheFriendsIds(userId1);
        var friendsUser2 = await _friendShipRepository.GetAllTheFriendsIds(userId2);

        // Calcular intersección (amigos en común)
        var commonFriendsCount = friendsUser1.Intersect(friendsUser2).Count();
        return commonFriendsCount;
    }

    public async Task<Result<List<UserWithCommonFriendCountDto>>> GetAllTheUsersThatAreNotFriends(string userId)
    {
        // IDs de usuarios que ya son amigos
        var usersIdToDiscard = new HashSet<string>(await _friendShipRepository.GetAllTheFriendsIds(userId));

        // IDs de usuarios con solicitudes pendientes (solicitadas o recibidas)
        var usersIdWithPendingRequests = await _friendShipRequestRepository.GetAllQueryable()
            .AsNoTracking()
            .Where(r => r.Status == FriendShipRequestStatus.Pending)
            .Where(r => r.RequestedByUserId == userId || r.TargetUserId == userId)
            .Select(r => r.RequestedByUserId == userId ? r.TargetUserId : r.RequestedByUserId)
            .ToListAsync();

        // Obtenemos usuarios activos
        var activeUsersResult = await _accountServiceForWebApp.GetAllUser(true);
        if (activeUsersResult.IsFailure || activeUsersResult.Value == null)
            return Result<List<UserWithCommonFriendCountDto>>.Fail("No se pudieron obtener los usuarios activos.");

        // Filtramos usuarios válidos: no amigos, sin solicitud pendiente, distinto al usuario actual
        var potentialUsers = activeUsersResult.Value
            .Where(u => 
                u.Id != userId &&
                !usersIdToDiscard.Contains(u.Id) &&
                !usersIdWithPendingRequests.Contains(u.Id)
            )
            .ToList();


        var potentialUsersWithCount = new List<UserWithCommonFriendCountDto>();

        foreach (var u in potentialUsers)
        {
            var commonCount = await GetCommonFriendsCountAsync(userId, u.Id);
            potentialUsersWithCount.Add(new UserWithCommonFriendCountDto
            {
                User = u,
                CommonFriendCount = commonCount
            });
        }
        
        return Result<List<UserWithCommonFriendCountDto>>.Ok(potentialUsersWithCount);
    }

    public async Task<Result<List<string>>> GetAllTheFriendsIds(string userId)
    {
        var friendsIds = await _friendShipRepository.GetAllTheFriendsIds(userId);
        return Result<List<string>>.Ok(friendsIds);
    }

    public async Task<Result<List<UserDto>>> GetAllTheFriends(string userId)
    {
        var friendsIds = await _friendShipRepository.GetAllTheFriendsIds(userId);
        var friends = await _accountServiceForWebApp.GetUsersByIds(friendsIds);
        return Result<List<UserDto>>.Ok(friends.Value!);
    }

    public async Task<Result> DeleteFrienship(string userId1, string userId2)
    {
        try
        {
            await _friendShipRepository.DeleteFriendShip(userId1, userId2);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(ex.Message);
        }
    }
}