using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.Viewmodels.User;

public class LoginViewModel
{
    [Required (ErrorMessage = "El campo nombre de usuario es requerido")]
    [DataType(DataType.Text)]
    public required string UserName { get; set; }
    [Required(ErrorMessage = "El campo contraseña es requerido")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}