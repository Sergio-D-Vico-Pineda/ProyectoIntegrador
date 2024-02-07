using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Integrador.Models
{
    public class Suministro
    {
        public int Id { get; set; }

        [Display(Name = "Proveedor")]
        [Required(ErrorMessage = "El proveedor es un campo obligatorio.")]
        public int ProveedorId { get; set; }

        [Display(Name = "Producto")]
        [Required(ErrorMessage = "El producto es un campo obligatorio.")]
        public int ProductoId { get; set; }

        private int unidades;

        [Required(ErrorMessage = "La cantidad es un campo obligatorio.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Debe contener solo números enteros positivos.")]
        [Range(1, double.MaxValue, ErrorMessage = "La cantidad tiene que ser mayor que 0.")]
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
