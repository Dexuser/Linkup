namespace LinkUp.Core.Persistence.Entities;

public class Comment
{
    public required int Id { get; set; }
    public required int PostId { get; set; }
    public required string Text { get; set; }
    
    public required string AuthorId { get; set; }
    public int? ParentCommentId { get; set; }
    
    public Comment? ParentComment { get; set; }
    public ICollection<Comment> Replies { get; set; } = [];
}