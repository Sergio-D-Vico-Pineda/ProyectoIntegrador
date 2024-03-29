﻿using System.ComponentModel.DataAnnotations;

namespace Integrador.Models
{
    public class Marca
    {
        public int Id { get; set; }

        [StringLength(30)]
        [Required(ErrorMessage = "El nombre es necesario.")]
        public required string Nombre { get; set; }

        public ICollection<Modelo>? Modelos { get; set; }
    }
}
