namespace LinkUp.Core.Application.Dtos.User;

public class UserSaveDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public string? Phone { get; set; }
    public string? PhotoPath { get; set; }
}