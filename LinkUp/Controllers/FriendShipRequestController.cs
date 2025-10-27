using System.Security.Claims;
using AutoMapper;
using LinkUp.Core.Application.Dtos.FriendShipRequest;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.FriendShipRequest;
using LinkUp.Core.Application.ViewModels.User;
using LinkUp.Core.Domain.Entities.Common;
using LinkUp.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class FriendShipRequestController : Controller
{
    private readonly IFriendShipRequestService _friendShipRequestService;
    private readonly IFriendShipService _friendShipService;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IMapper _mapper;

    public FriendShipRequestController(IFriendShipRequestService friendShipRequestService, IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp, IFriendShipService friendShipService)
    {
        _friendShipRequestService = friendShipRequestService;
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _friendShipService = friendShipService;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        var viewmodel = new FriendShipRequestHomeIndex();
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var requestResult = await _friendShipRequestService.GetAllTheRequestsOfThisUser(userId);
        
        if (requestResult.IsFailure)
        {
            this.SendValidationErrorMessages(requestResult);
            return View(viewmodel);
        }
        
        var requests = requestResult.Value;
        var toMe = requests.Where(r => r.TargetUserId == userId).ToList();
        var toOthers = requests.Where(r => r.RequestedByUserId == userId).ToList();
        viewmodel.FriendShipRequests = _mapper.Map<List<FriendShipRequestViewModel>>(toMe);
        viewmodel.FriendShipTargets = _mapper.Map<List<FriendShipRequestViewModel>>(toOthers);
        
        return View(viewmodel);
    }



    public async Task<IActionResult> Delete(int id, string userName)
    {
        return View(new FriendShipRequestDeleteViewModel
        {
            FriendShipRequestId = id,
            UserName = userName 
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> Delete(FriendShipRequestDeleteViewModel viewModel)
    {
        var deleteResult = await _friendShipRequestService.DeleteAsync(viewModel.FriendShipRequestId);
        if (deleteResult.IsFailure)
        {
            this.SendValidationErrorMessages(deleteResult);
            return View(viewModel);
        }
        return RedirectToRoute(new { controller="FriendShipRequest", action = "Index"});
        
    }

    public async Task<IActionResult> Create()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var userDtoResult = await _friendShipService.GetAllTheUsersThatAreNotFriends(userId);
        if (userDtoResult.IsFailure)
        {
            this.SendValidationErrorMessages(userDtoResult);
            RedirectToAction("Index");
        }
        var viewmodel = _mapper.Map<List<UserWithCommonFriendCountViewModel>>(userDtoResult.Value!); 
        return View(viewmodel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(string SelectedUserId)
    {
        string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var request = new FriendShipRequestDto
        {
            Id = 0,
            RequestedByUserId = currentUserId,
            TargetUserId = SelectedUserId,
            CommonFriendsCount = 0,
            CreatedAt = DateTime.Now,
            Status = FriendShipRequestStatus.Pending
        };
        var createResult = await _friendShipRequestService.AddAsync(request);
                
        if (createResult.IsFailure)
        {
            this.SendValidationErrorMessages(createResult);
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var userDtoResult = await _friendShipService.GetAllTheUsersThatAreNotFriends(userId);
            if (userDtoResult.IsFailure)
            {
                this.SendValidationErrorMessages(userDtoResult);
                RedirectToAction("Index");
            }
            var viewmodel = _mapper.Map<List<UserWithCommonFriendCountViewModel>>(userDtoResult.Value!); 
            return View(viewmodel);
        }
        
        return RedirectToRoute(new {controller="FriendShipRequest", action="Index"});
    }

    public IActionResult RespondRequest(int friendShipRequestId, string userName,  bool accepted)
    {
        return View(new ResponseFriendShipRequestViewModel
        {
            FriendShipRequestId = friendShipRequestId,
            UserName = userName,
            Accepted = accepted
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> RespondRequest(ResponseFriendShipRequestViewModel response)
    {
        var result = await _friendShipRequestService.RespondRequestAsync(response.FriendShipRequestId, response.Accepted);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(response);
        }
        return RedirectToRoute(new { controller = "FriendShipRequest", action = "Index" });
    }
}