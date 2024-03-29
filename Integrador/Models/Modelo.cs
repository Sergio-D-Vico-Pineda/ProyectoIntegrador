﻿using System.ComponentModel.DataAnnotations;

namespace Integrador.Models
{
    public class Modelo
    {
        public int Id { get; set; }

        [StringLength(30)]
        [Required(ErrorMessage = "El nombre es requerido.")]
        public required string Nombre { get; set; }

        [Display(Name = "Marca")]
        public required int MarcaId { get; set; }

        public Marca? Marca { get; set; }

        public ICollection<Producto>? Productos { get; set; }
    }
}
