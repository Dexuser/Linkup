using System.ComponentModel.DataAnnotations;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.ViewModels.User;

namespace LinkUp.Core.Application.ViewModels.Comment;

public class CommentCreateViewModel
{
    public required int PostId { get; set; }
    [Required (ErrorMessage = "El campo texto es requerido")]
    public required string Text { get; set; }
    
    [Required (ErrorMessage = "El campo AuthorId es requerido")] // es llenado por el software, no por el usuario
    public required string AuthorId { get; set; }
    public UserViewModel? Author { get; set; }
    public int? ParentCommentId { get; set; }
    
    public CommentDto? ParentComment { get; set; }
    public ICollection<CommentDto> Replies { get; set; } = [];
}