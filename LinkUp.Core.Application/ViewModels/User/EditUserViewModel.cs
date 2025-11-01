﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUp.Core.Application.ViewModels.User
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El campo nombre es requerido")]
        [DataType(DataType.Text)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "El campo apellido es requerido")]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }


        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Compare(nameof(Password),ErrorMessage = "Las contraseñas deben de coincidir")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Debes de proveer un numero de telefono")]
        public required string Phone { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? ProfileImageFile { get; set; }
    }
}
