﻿using System.ComponentModel.DataAnnotations;

namespace Integrador.Models
{
    public class Proveedor
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public required string Nombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public required string Email { get; set; }

        [StringLength(9, ErrorMessage = "El NIF debe tener una longitud de 9.")]
        [Display(Name = "NIF")]
        [Required(ErrorMessage = "El NIF es obligatorio.")]
        [RegularExpression(@"^\d{8}[A-Za-z]$", ErrorMessage = "El NIF no es válido.")]
        public required string Nif { get; set; }

        [StringLength(12)]
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [RegularExpression(@"^(\+34)?\(?([0-9]{3})\)?[-.●]?([0-9]{3})[-.●]?([0-9]{3})$", ErrorMessage = "El número de teléfono no es válido.")]
        public required string Telefono { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        public ICollection<Suministro>? Suministros { get; set; }
    }
}
