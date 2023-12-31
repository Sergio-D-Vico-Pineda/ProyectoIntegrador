// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Integrador.Views.MisDatos
{
    public class ChangePasswordModel
    {
        public ChangePasswordModel()
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña actual")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [StringLength(100, ErrorMessage = "La nueva contraseña debe tener al menos {2} caracteres y como máximo {1} caracteres de longitud.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Nueva contraseña")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación de contraseña no coinciden.")]
            public string ConfirmPassword { get; set; }
        }
    }
}
