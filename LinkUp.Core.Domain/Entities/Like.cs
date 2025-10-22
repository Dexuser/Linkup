namespace LinkUp.Core.Domain.Entities;

public class Like
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public required string UserId { get; set; }
    public required bool IsLiked { get; set; }
    
    public Post? Post { get; set; }
}