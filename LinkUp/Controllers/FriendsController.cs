using System.Security.Claims;
using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Like;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.Comment;
using LinkUp.Core.Application.ViewModels.Friends;
using LinkUp.Core.Application.ViewModels.Post;
using LinkUp.Core.Application.ViewModels.User;
using LinkUp.Helpers;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class FriendsController : Controller
{
    private readonly IAccountServiceForWebApp  _accountServiceForWebApp;
    private readonly IPostService _postService;
    private readonly ICommentService _commentService;
    private readonly ILikeService _likeService;
    private readonly IFriendShipService _friendShipService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public FriendsController(IPostService postService, ICommentService commentService, ILikeService likeService, IMapper mapper, UserManager<AppUser> userManager, ILogger<HomeController> logger, IFriendShipService friendShipService, IAccountServiceForWebApp accountServiceForWebApp)
    {
        _postService = postService;
        _commentService = commentService;
        _likeService = likeService;
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
        _friendShipService = friendShipService;
        _accountServiceForWebApp = accountServiceForWebApp;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = await CreateFriendsHomeIndexViewModel();
        return View(viewModel);
    }
    
    public async Task<IActionResult> Details(string userId)
    {
        string currentUserSession = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var user = await _accountServiceForWebApp.GetUserById(userId);
        var posts = await _postService.GetAllPostsOfThisUser(userId);
        var friends = await _friendShipService.GetAllTheFriends(currentUserSession);
        var viewModel = new FriendsIndexViewModel
        {
            Posts = _mapper.Map<List<PostViewModel>>(posts.Value),
            CommentCreateViewModel = new CommentCreateViewModel
            {
                PostId = 0,
                Text = "",
                AuthorId = ""
            },
            CommentEditViewModel = new CommentEditViewModel
            {
                Id = 0,
                Text = ""
            },
            Friends = _mapper.Map<List<UserViewModel>>(friends.Value),
            CurrentUser = _mapper.Map<UserViewModel>(user.Value) 
        };
        
        return View("Details",viewModel);
    }

    private void ShowPostModal()
    {
        ViewBag.ShowPostModal = true;
    }
    private void ShowCommentModal()
    {
        ViewBag.ShowCommentModal = true;
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment([Bind(Prefix = "CommentCreateViewModel")] CommentCreateViewModel createViewModel)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await CreateFriendsHomeIndexViewModel();
            viewModel.CommentCreateViewModel = createViewModel;
            return View("Index",viewModel);
        }
        
        var currentUser = await _userManager.GetUserAsync(User);
        
        var commentDto = _mapper.Map<CommentDto>(createViewModel);
        commentDto.AuthorId = currentUser!.Id;
        var result = await _commentService.AddAsync(commentDto);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            var viewModel = await CreateFriendsHomeIndexViewModel();
            viewModel.CommentCreateViewModel = createViewModel;
            return View("index", viewModel);
        }
        return RedirectToRoute(new {controller ="Friends" , action = "Index"});
    }

    private async Task<FriendsIndexViewModel> CreateFriendsHomeIndexViewModel()
    {
        
        string currentUserSession = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var user =  await _accountServiceForWebApp.GetUserById(currentUserSession);
        var friends = await _friendShipService.GetAllTheFriends(currentUserSession);
        var viewModel = new FriendsIndexViewModel
        {
            Posts = new List<PostViewModel>(),
            CommentCreateViewModel = new CommentCreateViewModel()
            {
                Text = "",
                AuthorId = "",
                PostId = 0,
                ParentCommentId = null
            },
            CommentEditViewModel = new CommentEditViewModel()
            {
                Id = 0,
                Text = "",
            },
            Friends = _mapper.Map<List<UserViewModel>>(friends.Value!),
            CurrentUser = _mapper.Map<UserViewModel>(user.Value),
        };
        
        var currentUser = await _userManager.GetUserAsync(User);
        var userResult = await _postService.GetAllFriendsPosts(currentUser!.Id);
        if (userResult.IsSuccess)
        {
            viewModel.Posts = _mapper.Map<List<PostViewModel>>(userResult.Value);
        }
        
        return viewModel;
    }
    
    
    [HttpPost]
    public async Task<IActionResult> EditComment([Bind(Prefix = "CommentEditViewModel")] CommentEditViewModel editViewModel)
    {
        if (!ModelState.IsValid)
        {
            ShowCommentModal();
            var viewModel = await CreateFriendsHomeIndexViewModel();
            viewModel.CommentEditViewModel = editViewModel;
            return View("Index",viewModel);
        }
        
        var commentResult = await _commentService.GetByIdAsync(editViewModel.Id);
        if (commentResult.IsFailure)
        {
            ShowCommentModal();
            this.SendValidationErrorMessages(commentResult);
            var viewModel = await CreateFriendsHomeIndexViewModel();
            viewModel.CommentEditViewModel = editViewModel;
            return View("index", viewModel);
        }
        
        var commentDto = commentResult.Value!;
        commentDto.Text = editViewModel.Text;
        var result = await _commentService.UpdateAsync(commentDto.Id,commentDto);
        
        if (result.IsFailure)
        {
            ShowCommentModal();
            this.SendValidationErrorMessages(result);
            var viewModel = await CreateFriendsHomeIndexViewModel();
            viewModel.CommentEditViewModel = editViewModel;
            return View("index", viewModel);
        }
        return RedirectToRoute(new {controller = "Friends", action = "Index"});
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var result = await _commentService.DeleteAsync(commentId);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            var viewModel = await CreateFriendsHomeIndexViewModel();
            return View("index", viewModel);
        }

        return RedirectToRoute(new {controller = "Friends", action = "Index"});
    }

    [HttpPost]
    public async Task<IActionResult> LikePost(int postId, bool like)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var likeResult = await  _likeService.GetThisLikeByPostIdAndUserId(postId, currentUser!.Id);
        
        if (likeResult.IsFailure)
        {
            this.SendValidationErrorMessages(likeResult);
            var viewModel = await CreateFriendsHomeIndexViewModel();
            return View("index", viewModel);
        }
        
        var likeDto = likeResult.Value;

        if (likeDto == null)
        {
            var createResult = await _likeService.AddAsync(new LikeDto
            {
                Id = 0,
                PostId = postId,
                IsLiked = like,
                UserId = currentUser.Id,
            });

            if (createResult.IsFailure)
            {
                this.SendValidationErrorMessages(createResult);
                var viewModel = await CreateFriendsHomeIndexViewModel();
                return View("index", viewModel);
            }
            return RedirectToRoute(new {controller = "Friends", action = "Index"});
        }

        likeDto.IsLiked = like;           
        var updateResult = await _likeService.UpdateAsync(likeDto.Id, likeDto);
        
        if (updateResult.IsFailure)
        {
            this.SendValidationErrorMessages(updateResult);
            var viewModel = await CreateFriendsHomeIndexViewModel();
            return View("index", viewModel);
        }
        return RedirectToRoute(new {controller = "Friends", action = "Index"});
    }

    [HttpPost]
    public async Task<IActionResult> DeleteFriend(string friendUserId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var deleteResult = await _friendShipService.DeleteFrienship(currentUserId, friendUserId);
        if (deleteResult.IsFailure)
        {
            this.SendValidationErrorMessages(deleteResult);
            var viewmodel = await CreateFriendsHomeIndexViewModel();
            return View("index",  viewmodel);
        }
        return RedirectToRoute(new {controller = "Friends", action = "Index"});
    }
    
    public IActionResult Privacy()
    {
        return View();
    }
}