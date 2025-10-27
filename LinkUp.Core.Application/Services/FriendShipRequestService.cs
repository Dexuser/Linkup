using AutoMapper;
using LinkUp.Core.Application.Dtos.FriendShip;
using LinkUp.Core.Application.Dtos.FriendShipRequest;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Entities;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Core.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Core.Application.Services;

public class FriendShipRequestService : GenericServices<FriendShipRequest, FriendShipRequestDto>,
    IFriendShipRequestService
{
    private readonly IFriendShipRequestRepository _friendShipRequestRepository;
    private readonly IFriendShipRepository _friendShipRepository;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    
    private readonly IFriendShipService _friendShipService;
    private readonly IMapper _mapper;

    public FriendShipRequestService(IFriendShipRequestRepository repository, IMapper mapper,
        IAccountServiceForWebApp accountServiceForWebApp, IFriendShipRepository friendShipRepository, IFriendShipService friendShipService) : base(repository,
        mapper)
    {
        _friendShipRequestRepository = repository;
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _friendShipRepository = friendShipRepository;
        _friendShipService = friendShipService;
    }

    public override async Task<Result<FriendShipRequestDto>> AddAsync(FriendShipRequestDto dtoModel)
    {
        bool thereIsADuplicate  = await _friendShipRequestRepository.GetAllQueryable()
            .AsNoTracking()
            .AnyAsync(fr => 
                fr.RequestedByUserId == dtoModel.RequestedByUserId && fr.TargetUserId == dtoModel.TargetUserId 
                ||
                fr.RequestedByUserId ==  dtoModel.TargetUserId && fr.TargetUserId == dtoModel.RequestedByUserId
                );

        if (thereIsADuplicate)
        {
            return Result<FriendShipRequestDto>.Fail("Ya existe una solicitud entre estos dos usuarios");
        }
        dtoModel.CreatedAt = DateTime.Now;
        return await base.AddAsync(dtoModel);
    }
    public async Task<Result<List<FriendShipRequestDto>>> GetAllTheRequestsOfThisUser(string userId)
    {
        try
        {
            var requests = await _friendShipRequestRepository.GetAllQueryable().AsNoTracking()
                .Where(r => r.Status == FriendShipRequestStatus.Pending)
                .Where(r => r.RequestedByUserId == userId || r.TargetUserId == userId)
                .ToListAsync();
            
            var userIds = requests
                .Select(r => r.RequestedByUserId == userId ? r.TargetUserId : r.RequestedByUserId)
                .Distinct()
                .ToList();
            
            userIds.Add(userId);
            var usersResult = await _accountServiceForWebApp.GetUsersByIds(userIds);
            var users = usersResult.Value!.ToDictionary(u => u.Id, u => u);

            var requestsDtos = _mapper.Map<List<FriendShipRequestDto>>(requests);

            foreach (var requestDto in requestsDtos)
            {
                requestDto.RequestedByUser = users.GetValueOrDefault(requestDto.RequestedByUserId);
                requestDto.TargetUser = users.GetValueOrDefault(requestDto.TargetUserId);

                var otherUserId = requestDto.RequestedByUserId == userId
                    ? requestDto.TargetUserId
                    : requestDto.RequestedByUserId;

                requestDto.CommonFriendsCount = await _friendShipService.GetCommonFriendsCountAsync(userId, otherUserId);
            }

            return Result<List<FriendShipRequestDto>>.Ok(requestsDtos);
        }
        catch (Exception e)
        {
            return Result<List<FriendShipRequestDto>>.Fail(e.Message);
        }
    }



    public async Task<Result> RespondRequestAsync(int friendShipRequestId, bool accept)
    {
        try
        {
            var request = await _friendShipRequestRepository.GetByIdAsync(friendShipRequestId);
            if (request == null)
            { 
                return Result.Fail("No existe ninguna solicitud con dicho ID");
            }
            await _friendShipRequestRepository.RespondRequestAsync(friendShipRequestId, accept);

            if (accept)
            {
                var createResult = await _friendShipService.AddAsync(new FriendShipDto
                {
                    UserId1 = request.RequestedByUserId,
                    UserId2 = request.TargetUserId,
                });

                if (createResult.IsFailure)
                {
                    return Result.Fail(createResult.GeneralError!);
                }
            }
            
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(e.Message);
        }
    }
}