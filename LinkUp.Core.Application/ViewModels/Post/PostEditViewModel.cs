using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUp.Core.Application.ViewModels.Post;

public class PostEditViewModel : IValidatableObject
{
    public required int Id { get; set; }
    
    [Required (ErrorMessage = "El campo texto es requerido")]
    [DataType(DataType.Text)]
    public required string Text { get; set; }
    
    [DataType(DataType.Upload)]
    public IFormFile? ImageFile { get; set; }
    
    
    [DataType(DataType.Text)]
    public string? VideoUrl { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ImageFile != null && !string.IsNullOrEmpty(VideoUrl))
        {
            yield return new ValidationResult(
                "Solo puedes llenar uno de los dos campos: Imagen o Video URL.",
                new[] { nameof(ImageFile),  nameof(VideoUrl) });
        }
    }
}