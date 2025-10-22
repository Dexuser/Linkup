namespace LinkUp.Core.Domain.Entities;

public class Post
{
    public required int Id { get; set; }
    public required string UserId { get; set; }
    public required string Text { get; set; }
    public string? ImagePath { get; set; }
    public string? VideoUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<Comment> Comments { get; set; } = []; // deberias de usar el Include
}