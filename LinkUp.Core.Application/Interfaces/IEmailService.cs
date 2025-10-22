using LinkUp.Core.Application.Dtos.Email;

namespace LinkUp.Core.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(EmailRequestDto emailRequestDto); 
}