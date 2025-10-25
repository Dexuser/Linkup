using AutoMapper;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.Viewmodels.User;
using LinkUp.Helpers;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers;

public class LoginController : Controller
{
    
    private readonly IMapper _mapper;
    private readonly IAccountServiceForWebApp _accountServiceForWebApp;
    private readonly UserManager<AppUser> _userManager;

    public LoginController(IMapper mapper, IAccountServiceForWebApp accountServiceForWebApp, UserManager<AppUser> userManager)
    {
        _mapper = mapper;
        _accountServiceForWebApp = accountServiceForWebApp;
        _userManager = userManager;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        AppUser? userSession = await _userManager.GetUserAsync(User);

        if (userSession != null)
        {
            return RedirectToRoute(new { controller = "Home", action = "Index" });
        }
        
        
        return View(new LoginViewModel() {UserName = "",  Password = ""});
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Index",loginViewModel);
        }
        
        var loginDto = _mapper.Map<LoginDto>(loginViewModel);
        var result = await _accountServiceForWebApp.AuthenticateAsync(loginDto);
        
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View("Index",loginViewModel);
        }
        return RedirectToRoute(new { controller = "Home", action = "Index" });
    }

    public async Task<IActionResult> Logout()
    {
        await _accountServiceForWebApp.SignOutAsync();
        return RedirectToRoute(new { controller = "Login", action = "Index" });
    }

    public IActionResult Register()
    {
        var vm = new RegisterUserViewModel
        {
            FirstName = "",
            LastName = "",
            Email = "",
            UserName = "",
            Password = "",
            ConfirmPassword = "",
            Phone = ""
        };

        return View(vm);
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterUserViewModel registerViewModel) 
    {
        if (!ModelState.IsValid)
        {
            return View(registerViewModel);
        }
        
        var saveDto = _mapper.Map<UserSaveDto>(registerViewModel);
        string origin = Request.Headers.Origin.FirstOrDefault() ?? "";
        var result = await _accountServiceForWebApp.RegisterUser(saveDto, origin);
        
        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(registerViewModel);
        }
        UserSaveDto returnUser =_mapper.Map<UserSaveDto>( result.Value!);
        returnUser.PhotoPath = FileManager.Upload(registerViewModel.ProfileImageFile, returnUser.Id, "users");
        await _accountServiceForWebApp.EditUser(returnUser, origin, true);
        
        return RedirectToRoute(new {controller = "Login", action = "Index"});
    }
    
    public async Task<IActionResult> ConfirmEmail(string userId,string token)
    {
        var result = await _accountServiceForWebApp.ConfirmAccountAsync(userId, token);
        if (result.IsFailure)
        {
            return View("ConfirmEmail", result.GeneralError);
        }
        return View("ConfirmEmail", "Tu cuenta ha sido activada correctamente. Ya puedes iniciar sesión");
    }

    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordRequestViewModel() {UserName = ""} );
    }
    
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }
        string origin = Request?.Headers?.Origin.ToString() ?? string.Empty;

        ForgotPasswordRequestDto dto = new() { UserName = vm.UserName,Origin = origin};

        var result = await _accountServiceForWebApp.ForgotPasswordAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(vm);
        }          

        return RedirectToRoute(new { controller = "Login", action = "Index" });
    }

    public IActionResult ResetPassword(string userId, string token)
    {           
        return View(new ResetPasswordRequestViewModel()
        {
            Id = userId,
            Token = token,
            Password = "",
            ConfirmPassword = "",
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }  

        ResetPasswordRequestDto dto = new()
        {
            Id = vm.Id,
            Password = vm.Password,
            Token = vm.Token
        };

        var result = await _accountServiceForWebApp.ResetPasswordAsync(dto);

        if (result.IsFailure)
        {
            this.SendValidationErrorMessages(result);
            return View(vm);
        }

        return RedirectToRoute(new { controller = "Login", action = "Index" });
    }
}