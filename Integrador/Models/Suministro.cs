﻿using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Integrador.Models
{
    public class Suministro
    {
        public int Id { get; set; }

        [Display(Name = "Proveedor")]
        public int ProveedorId { get; set; }

        [Display(Name = "Id del Producto")]
        public int ProductoId { get; set; }

        private int unidades;

        [Required(ErrorMessage = "La cantidad es un campo obligatorio.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Debe contener solo números.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad tiene que ser mayor que 0.")]
        public int Unidades
        {
            get
            {
                return unidades;
            }
            set
            {
                if (value >= 0)
                    unidades = value;
            }
        }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de suministro")]
        public DateTime? FechaSuministro { get; set; } = DateTime.Now; // Fecha actual por defecto

        public virtual Proveedor? Proveedor { get; set; }
        public virtual Producto? Producto { get; set; }
    }
}
