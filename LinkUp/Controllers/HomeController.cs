using System.Diagnostics;
using AutoMapper;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Dtos.Post;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.Viewmodels.Comment;
using LinkUp.Core.Application.Viewmodels.HomeIndex;
using LinkUp.Core.Application.Viewmodels.Post;
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
    private readonly IPostService _postService;
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<HomeController> _logger;
    

    public HomeController(ILogger<HomeController> logger, IPostService postService, UserManager<AppUser> userManager, IMapper mapper, ICommentService commentService)
    {
        _logger = logger;
        _postService = postService;
        _userManager = userManager;
        _mapper = mapper;
        _commentService = commentService;
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
    public async Task<IActionResult> CreateComment([Bind(Prefix = "CommentCreateViewModel")] CommentCreateViewModel createViewModel)
    {
        var commentDto = _mapper.Map<CommentDto>(createViewModel);
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

    private async Task<HomeIndexViewModel> CreateHomeIndexViewModel()
    {
        var viewModel = new HomeIndexViewModel()
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
            }
            
        };
        
        var currentUser = await _userManager.GetUserAsync(User);
        var userResult = await _postService.GetAllPostsOfThisUser(currentUser!.Id);
        if (userResult.IsSuccess)
        {
            viewModel.Posts = _mapper.Map<List<PostViewModel>>(userResult.Value);
        }
        
        return viewModel;
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
}