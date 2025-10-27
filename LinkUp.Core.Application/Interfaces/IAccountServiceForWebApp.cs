using LinkUp.Core.Application.Dtos.User;

namespace LinkUp.Core.Application.Interfaces;

public interface IAccountServiceForWebApp
{
    Task<Result<UserDto>> AuthenticateAsync(LoginDto loginDto);
    Task SignOutAsync();
    Task<Result<UserDto>> RegisterUser(UserSaveDto saveDto, string origin);
    Task<Result<UserDto>> EditUser(UserSaveDto saveDto, string origin,bool? isCreated = false);
    Task<Result> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<Result> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<Result> DeleteAsync(string id);
    Task<Result<UserDto>> GetUserByEmail(string email);
    Task<Result<UserDto>> GetUserById(string id);
    Task<Result<List<UserDto>>> GetUsersByIds(List<string> ids);
    Task<Result<UserDto>> GetUserByUserName(string userName);
    Task<Result<List<UserDto>>> GetAllUser(bool? isActive = true);
    Task<Result> ConfirmAccountAsync(string userId, string token);
}