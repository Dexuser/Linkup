using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Comment;

public class CommentEditViewModel
{
    public required int Id { get; set; }
    [Required (ErrorMessage = "El campo texto es requerido")]
    public required string Text { get; set; }
}