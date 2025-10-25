using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.Viewmodels.User
{
    public class ForgotPasswordRequestViewModel
    {
        [Required(ErrorMessage = "El campo nombre de usuario es requerido")]
        [DataType(DataType.Text)]
        public required string UserName { get; set; }      
    }
}
