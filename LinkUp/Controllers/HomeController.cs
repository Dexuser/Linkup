using System.Diagnostics;
using System.Security.Claims;
using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Like;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.Comment;
using LinkUp.Core.Application.ViewModels.HomeIndex;
using LinkUp.Core.Application.ViewModels.Post;
using LinkUp.Core.Application.ViewModels.User;
using LinkUp.Helpers;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Mvc;
using LinkUp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace LinkUp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly IPostService _postService;
    private readonly ICommentService _commentService;
    private readonly ILikeService _likeService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<HomeController> _logger;
    

    public HomeController(ILogger<HomeController> logger, IPostService postService, UserManager<AppUser> userManager, IMapper mapper, ICommentService commentService, ILikeService likeService, IAccountServiceForWebApp accountServiceForWebApp)
    {
        _logger = logger;
        _postService = postService;
        _userManager = userManager;
        _mapper = mapper;
        _commentService = commentService;
        _likeService = likeService;
        _accountServiceForWebApp = accountServiceForWebApp;
    }

    public async Task<IActionResult> Index()
    {
        var viewModel = await CreateHomeIndexViewModel();
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Post([Bind(Prefix = "PostCreateViewModel")] PostCreateViewModel createViewModel)
    {
        if (!ModelState.IsValid)
        {
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.PostCreateViewModel = createViewModel;
            return View("Index",viewModel);
        }
        
        var currentUser = await _userManager.GetUserAsync(User);
        
        var postDto = _mapper.Map<PostDto>(createViewModel);
        postDto.CreatedAt = DateTime.Now;
        postDto.UserId = currentUser!.Id;
        if (!string.IsNullOrEmpty(createViewModel.VideoUrl))
        {
            postDto.VideoUrl = VideoUrlHelper.NormalizeYouTubeUrl( createViewModel.VideoUrl);
        }
        
        var postResult = await _postService.AddAsync(postDto);
        
        if (postResult.IsFailure)
        {
            this.SendValidationErrorMessages(postResult);
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.PostCreateViewModel = createViewModel;
            return View("index", viewModel);
        }

        if (createViewModel.ImageFile != null)
        {
            
            var returnPost = postResult.Value!;
            returnPost.ImagePath = FileManager.Upload(createViewModel.ImageFile, returnPost.Id.ToString()!, "imagesPosts" );
            var updateResult = await _postService.UpdateAsync(returnPost.Id, returnPost);
            
            if (updateResult.IsFailure)
            {
                this.SendValidationErrorMessages(updateResult);
                var viewModel = await CreateHomeIndexViewModel();
                viewModel.PostCreateViewModel = createViewModel;
                return View("index", viewModel);
            }
        }
        return RedirectToRoute(new {controller ="Home" , action = "Index"});
    }
    
    [HttpPost]
    public async Task<IActionResult> EditPost([Bind(Prefix = "PostEditViewModel")] PostEditViewModel editViewModel)
    {
        if (!ModelState.IsValid)
        {
            ShowPostModal();
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.PostEditViewModel = editViewModel;
            return View("Index",viewModel);
        }
        
        var postResult = await _postService.GetByIdAsync(editViewModel.Id);
        
        if (postResult.IsFailure)
        {
            ShowPostModal();
            this.SendValidationErrorMessages(postResult);
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.PostEditViewModel = editViewModel;
            return View("index", viewModel);
        }
        
        var postDto = postResult.Value!;
        postDto.Text = editViewModel.Text; 

        if (!string.IsNullOrEmpty(editViewModel.VideoUrl))
        {
            postDto.VideoUrl = VideoUrlHelper.NormalizeYouTubeUrl( editViewModel.VideoUrl);
            FileManager.DeleteFile(postDto.ImagePath!);
            postDto.ImagePath = null;
        }
        
        if (editViewModel.ImageFile != null)
        {
            postDto.ImagePath = FileManager.Upload(editViewModel.ImageFile, postDto.Id.ToString()!, "imagesPosts", true, postDto.ImagePath);
            postDto.VideoUrl = null;
        }
        var updateResult = await _postService.UpdateAsync(postDto.Id, postDto);
            
        if (updateResult.IsFailure)
        {
            ShowPostModal();
            this.SendValidationErrorMessages(updateResult);
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.PostEditViewModel = editViewModel;
            return View("index", viewModel);
        }

        return RedirectToRoute(new {controller ="Home" , action = "Index"});
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
            var viewModel = await CreateHomeIndexViewModel();
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
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.CommentCreateViewModel = createViewModel;
            return View("index", viewModel);
        }
        
        return RedirectToRoute(new {controller ="Home" , action = "Index"});
    }


    
    [HttpPost]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var result = await _postService.DeleteAsync(postId);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            var viewModel = await CreateHomeIndexViewModel();
            return View("index", viewModel);
        }

        return RedirectToRoute(new {controller = "Home", action = "Index"});
    }
    
    [HttpPost]
    public async Task<IActionResult> EditComment([Bind(Prefix = "CommentEditViewModel")] CommentEditViewModel editViewModel)
    {
        if (!ModelState.IsValid)
        {
            ShowCommentModal();
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.CommentEditViewModel = editViewModel;
            return View("Index",viewModel);
        }
        
        var commentResult = await _commentService.GetByIdAsync(editViewModel.Id);
        if (commentResult.IsFailure)
        {
            ShowCommentModal();
            this.SendValidationErrorMessages(commentResult);
            var viewModel = await CreateHomeIndexViewModel();
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
            var viewModel = await CreateHomeIndexViewModel();
            viewModel.CommentEditViewModel = editViewModel;
            return View("index", viewModel);
        }
        return RedirectToRoute(new {controller = "Home", action = "Index"});
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var result = await _commentService.DeleteAsync(commentId);
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            var viewModel = await CreateHomeIndexViewModel();
            return View("index", viewModel);
        }

        return RedirectToRoute(new {controller = "Home", action = "Index"});
    }

    [HttpPost]
    public async Task<IActionResult> LikePost(int postId, bool like)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        var likeResult = await  _likeService.GetThisLikeByPostIdAndUserId(postId, currentUser!.Id);
        
        if (likeResult.IsFailure)
        {
            this.SendValidationErrorMessages(likeResult);
            var viewModel = await CreateHomeIndexViewModel();
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
                var viewModel = await CreateHomeIndexViewModel();
                return View("index", viewModel);
            }
            return RedirectToRoute(new {controller = "Home", action = "Index"});
        }

        likeDto.IsLiked = like;           
        var updateResult = await _likeService.UpdateAsync(likeDto.Id, likeDto);
        
        if (updateResult.IsFailure)
        {
            this.SendValidationErrorMessages(updateResult);
            var viewModel = await CreateHomeIndexViewModel();
            return View("index", viewModel);
        }
        return RedirectToRoute(new {controller = "Home", action = "Index"});
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    private async Task<HomeIndexViewModel> CreateHomeIndexViewModel()
    {
        var userid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var user = await _accountServiceForWebApp.GetUserById(userid);
        
        var viewModel = new HomeIndexViewModel
        {
            Posts = new List<PostViewModel>(),
            PostCreateViewModel = new PostCreateViewModel()
            {
                Text = "",
                ImageFile = null,
                VideoUrl = null
            },
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
            PostEditViewModel = new PostEditViewModel()
            {
                Id = 0,
                Text = "",
                ImageFile = null,
                VideoUrl = ""
            },
            CurrentUser = _mapper.Map<UserViewModel>(user.Value!) 
        };
        
        var currentUser = await _userManager.GetUserAsync(User);
        var userResult = await _postService.GetAllPostsOfThisUser(currentUser!.Id);
        if (userResult.IsSuccess)
        {
            viewModel.Posts = _mapper.Map<List<PostViewModel>>(userResult.Value);
        }
        
        return viewModel;
    }
}