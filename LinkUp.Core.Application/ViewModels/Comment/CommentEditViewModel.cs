using System.ComponentModel.DataAnnotations;
using LinkUp.Core.Application.Dtos.Comment;
using LinkUp.Core.Application.Viewmodels.User;

namespace LinkUp.Core.Application.Viewmodels.Comment;

public class CommentEditViewModel
{
    public required int Id { get; set; }
    [Required (ErrorMessage = "El campo texto es requerido")]
    public required string Text { get; set; }
}