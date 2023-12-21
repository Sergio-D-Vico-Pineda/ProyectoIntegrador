using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "La cantidad es un campo obligatorio.")]
        public required int Unidades { get; set; }

        [Display(Name = "Fecha de suministro")]
        [DataType(DataType.Date)]
        public DateTime? FechaSuministro { get; set; } = DateTime.Now;

        public Proveedor? Proveedor { get; set; }
        public Producto? Producto { get; set; }
    }
}
