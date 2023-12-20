using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Integrador.Models
{
    public class Suministro
    {
        private int unidades;

        public int Id { get; set; }

        public int ProveedorId { get; set; }

        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es un campo obligatorio.")]
        public required int Unidades { get; set; }

        [Display(Name = "Fecha de suministro")]
        [Required(ErrorMessage = "La fecha del suministro es un campo obligatorio.")]
        [DataType(DataType.Date)]
        public DateTime FechaSuministro { get; set; } = DateTime.Now;

        public Proveedor? Proveedor { get; set; }
        public Producto? Producto { get; set; }
    }
}
