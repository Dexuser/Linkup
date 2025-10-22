using System.Text;
using LinkUp.Core.Application;
using LinkUp.Core.Application.Dtos;
using LinkUp.Core.Application.Dtos.Email;
using LinkUp.Core.Application.Dtos.User;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApp : IAccountServiceForWebApp
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        
        public AccountServiceForWebApp(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }
        
        // En el ModelState, los Empty strings serán los errores generales
        public async Task<Result<UserDto>> AuthenticateAsync(LoginDto loginDto)
        {
            Dictionary<string, List<string>> errors = new();
            
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user == null)
            {
                return Result<UserDto>.Fail( $"There is no account registered with this username: {loginDto.UserName}");
            }

            if (!user.EmailConfirmed)
            {
                return Result<UserDto>.Fail($"This account {loginDto.UserName} is not active, you should check your email");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", loginDto.Password, false, true);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    string error =
                        $"Your account {loginDto.UserName} has been locked due to multiple failed attempts." +
                        $" Please try again in 10 minutes. If you don’t remember your password, " +
                        $"you can go through the password reset process.";
                
                    return Result<UserDto>.Fail(error);
                }
                
                return Result<UserDto>.Fail($"these credentials are invalid for this user: {user.UserName}");
            }

            //var rolesList = await _userManager.GetRolesAsync(user);
            var userDto = new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName,
                IsVerified = user.EmailConfirmed,
                Phone = user.PhoneNumber,
                PhotoPath = user.PhotoPath
            };

            return Result<UserDto>.Ok(userDto);
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        
        public async Task<Result<UserDto>> RegisterUser(UserSaveDto saveDto, string origin)
        {
            Dictionary<string, List<string>> errors = new();
            
            var userWithSameUserName = await _userManager.FindByNameAsync(saveDto.UserName);
            if (userWithSameUserName != null)
            {
                
                return Result<UserDto>.Fail($"this username: {saveDto.UserName} is already taken.");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(saveDto.Email);
            if (userWithSameEmail != null)
            {
                return Result<UserDto>.Fail($"this email: {saveDto.Email} is already taken.");
            }

            AppUser user = new AppUser
            {
                FirstName = saveDto.FirstName,
                LastName = saveDto.LastName,
                Email = saveDto.Email,
                UserName = saveDto.UserName,
                PhotoPath = saveDto.PhotoPath,
                EmailConfirmed = false,
                PhoneNumber = saveDto.Phone,
            };

            var result = await _userManager.CreateAsync(user, saveDto.Password);
            if (!result.Succeeded)
            {
                errors[""] = result.Errors.Select(s => s.Description).ToList();
                return Result<UserDto>.Fail(errors);
            }
            
            //await _userManager.AddToRoleAsync(user, saveDto.Role);
            string verificationUri = await GetVerificationEmailUri(user, origin);
            await _emailService.SendAsync(new EmailRequestDto()
            {
                To = saveDto.Email,
                HtmlBody = $"Please confirm your account visiting this URL {verificationUri}",
                Subject = "Confirm registration"
            });

            //var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName,
                IsVerified = user.EmailConfirmed,
                Phone = user.PhoneNumber,
                PhotoPath = user.PhotoPath
            };
            
            return Result<UserDto>.Ok(userDto);
        }
        
        
        public async Task<Result<UserDto>> EditUser(UserSaveDto saveDto, string origin,bool? isCreated = false)
        {
            Dictionary<string, List<string>> errors = new();
            bool isNotcreated = !isCreated ?? false;

            var userWithSameUserName = await _userManager.Users.FirstOrDefaultAsync(w => w.UserName == saveDto.UserName && w.Id != saveDto.Id);
            if (userWithSameUserName != null)
            {
                return Result<UserDto>.Fail($"this username: {saveDto.UserName} is already taken.");
                
            }

            var userWithSameEmail = await _userManager.Users.FirstOrDefaultAsync(w => w.Email == saveDto.Email && w.Id != saveDto.Id);
            if (userWithSameEmail != null)
            {
                return Result<UserDto>.Fail($"this email: {saveDto.Email} is already taken.");
            }

            var user = await _userManager.FindByIdAsync(saveDto.Id);

            if (user == null)
            {
                errors[""] = new List<string>() {  };
                return Result<UserDto>.Fail( $"There is no account registered with this user");
            }
            

            user.FirstName = saveDto.FirstName;
            user.LastName = saveDto.LastName;
            user.UserName = saveDto.UserName;
            user.PhotoPath = string.IsNullOrWhiteSpace(saveDto.PhotoPath) ? user.PhotoPath : saveDto.PhotoPath;
            user.EmailConfirmed = user.EmailConfirmed && user.Email == saveDto.Email;
            user.Email = saveDto.Email;
            user.PhoneNumber = saveDto.Phone;

            if (!string.IsNullOrWhiteSpace(saveDto.Password) && isNotcreated)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultChange = await _userManager.ResetPasswordAsync(user, token, saveDto.Password);

                if (!resultChange.Succeeded)
                {
                    errors[""] = resultChange.Errors.Select(s => s.Description).ToList();
                    return Result<UserDto>.Fail(errors);
                }
            }

            var result = await _userManager.UpdateAsync(user);
            
            if (!result.Succeeded)
            {
                errors[""] = result.Errors.Select(s => s.Description).ToList();
                return Result<UserDto>.Fail(errors);
            }
           
            //var rolesList = await _userManager.GetRolesAsync(user);
            //await _userManager.RemoveFromRolesAsync(user, rolesList.ToList());
            //await _userManager.AddToRoleAsync(user, saveDto.Role);

            if (!user.EmailConfirmed && isNotcreated)
            {
                string verificationUri = await GetVerificationEmailUri(user, origin);
                await _emailService.SendAsync(new EmailRequestDto()
                {
                    To = saveDto.Email,
                    HtmlBody = $"Please confirm your account visiting this URL {verificationUri}",
                    Subject = "Confirm registration"
                });
            }             

            //var updatedRolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName,
                IsVerified = user.EmailConfirmed,
                Phone = user.PhoneNumber,
                PhotoPath = user.PhotoPath
            };

            return Result<UserDto>.Ok(userDto);
        }

        public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            
            Dictionary<string, List<string>> errors = new();

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return Result.Fail($"There is no account registered with this username {request.UserName}");
            }

            var resetUri = await GetResetPasswordUri(user, request.Origin);
            
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            await _emailService.SendAsync(new EmailRequestDto()
            {
                To = user.Email,
                HtmlBody = $"Please reset your password account visiting this URL {resetUri}",
                Subject = "Reset password"
            });

            return Result.Ok();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            Dictionary<string, List<string>> errors = new();
            var user = await _userManager.FindByIdAsync(request.Id);
            
            if (user == null)
            {
                return Result.Fail($"There is no account registered with this user");
            }

            var token= Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var result = await _userManager.ResetPasswordAsync(user, token, request.Password);
            
            if (!result.Succeeded)
            {
                errors[""] = result.Errors.Select(s => s.Description).ToList();
                return Result.Fail(errors);
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return Result.Ok();
        }
        public async Task<Result> DeleteAsync(string id)
        {
            Dictionary<string, List<string>> errors = new();
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Result.Fail($"There is no account registered with this user");
            }
            await _userManager.DeleteAsync(user);

            return Result.Ok();
        }
        
        public async Task<Result<UserDto>> GetUserByEmail(string email)
        {
            Dictionary<string, List<string>> errors = new();
            var user = await _userManager.FindByEmailAsync(email);

           if (user == null)
            {
                return Result<UserDto>.Fail("There is no account registered with this user");
            }

            //var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName,
                IsVerified = user.EmailConfirmed,
                Phone = user.PhoneNumber,
                PhotoPath = user.PhotoPath
            };
            return Result<UserDto>.Ok(userDto);
        }
        
        public async Task<Result<UserDto>> GetUserById(string id)
        {
            Dictionary<string, List<string>> errors = new();
            var user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return Result<UserDto>.Fail("There is no account registered with this user");
            }

            //var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName,
                IsVerified = user.EmailConfirmed,
                Phone = user.PhoneNumber,
                PhotoPath = user.PhotoPath
            };
            
            return Result<UserDto>.Ok(userDto);
        }
        
        public async Task<Result<UserDto>> GetUserByUserName(string userName)
        {
            Dictionary<string, List<string>> errors = new();
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return Result<UserDto>.Fail("There is no account registered with this user");
            }

            var rolesList = await _userManager.GetRolesAsync(user);

            var userDto = new UserDto()
            {
                Id = user.Id,
                Email = user.Email ?? "",
                UserName = user.UserName ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName,
                IsVerified = user.EmailConfirmed,
                Phone = user.PhoneNumber,
                PhotoPath = user.PhotoPath
            };

            return Result<UserDto>.Ok(userDto);
        }
        public async Task<List<UserDto>> GetAllUser(bool? isActive = true)
        {
            List<UserDto> listUsersDtos = [];

            var users = _userManager.Users;

            if (isActive != null && isActive == true)
            {
                users = users.Where(w => w.EmailConfirmed);
            }

            var listUser = await users.ToListAsync();

            foreach (var user in listUser)
            {
                //var roleList = await _userManager.GetRolesAsync(item);
                listUsersDtos.Add(new UserDto()
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    UserName = user.UserName ?? "",
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName,
                    IsVerified = user.EmailConfirmed,
                    Phone = user.PhoneNumber,
                    PhotoPath = user.PhotoPath
                });
            }

            return listUsersDtos;
        }
        public async Task<Result> ConfirmAccountAsync(string userId, string token)
        {
            Dictionary<string, List<string>> errors = new();
            var user = await _userManager.FindByIdAsync(userId);
            
            if (user == null)
            {
                return Result<UserDto>.Fail("There is no account registered with this user");
            }


            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, token);
            
            if (!result.Succeeded)
            {

                return Result<UserDto>.Fail($"An error occurred while confirming this email {user.Email}");
            }
            
            return Result.Ok();
        }

        #region "Private methods"

        private async Task<string> GetVerificationEmailUri(AppUser user, string origin)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var route = "Login/ConfirmEmail";
            var completeUrl = new Uri(string.Concat(origin, "/", route));// origin = https://localhost:58296 route=Login/ConfirmEmail
            var verificationUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri.ToString(), "token", token);

            return verificationUri;
        }
        private async Task<string> GetResetPasswordUri(AppUser user, string origin)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var route = "Login/ResetPassword";
            var completeUrl = new Uri(string.Concat(origin, "/", route));// origin = https://localhost:58296 route=Login/ConfirmEmail
            var resetUri = QueryHelpers.AddQueryString(completeUrl.ToString(), "userId", user.Id);
            resetUri = QueryHelpers.AddQueryString(resetUri.ToString(), "token", token);

            return resetUri;
        }
        #endregion
    }
}
