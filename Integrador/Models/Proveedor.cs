﻿using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;

namespace Integrador.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public required string Nombre { get; set; }

        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public required string Email { get; set; }

        [Display(Name = "NIF")]
        [Required(ErrorMessage = "El NIF es obligatorio.")]
        /*[RegularExpression(@"^\d{8}[A-Za-z]$", ErrorMessage = "El NIF no es válido.")]*/
        public required string Nif { get; set; }

        [StringLength(12)]
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public required string Telefono { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        public ICollection<Suministro>? Suministros { get; set; }
    }
}
