using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class DetallePedido1
    {
        public int Id { get; set; }

        [Display(Name = "Número de pedido")]
        public required int PedidoId { get; set; }

        [Display(Name = "Id del producto")]
        public required int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        public required int Cantidad { get; set; }

        [Display(Name = "Precio Unitario")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnidad { get; set; } // Mayor que 0

        [Range(0.1, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$",
            ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        public required string PrecioUnidadCadena
        {
            get
            {
                return Convert.ToString(PrecioUnidad).Replace(',', '.');
            }
            set
            {
                PrecioUnidad = Convert.ToDecimal(value.Replace('.', ','));
            }
        } // Mayor que 0

        [Display(Name = "Precio Total")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioTotal { get; set; }

        [Range(0.1, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$",
            ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        public required string PrecioTotalCadena
        {
            get
            {
                return Convert.ToString(PrecioTotal).Replace(',', '.');
            }
            set
            {
                PrecioTotal = Convert.ToDecimal(value.Replace('.', ','));
            }
        }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Descuento { get; set; }

        public virtual Producto? Producto { get; set; }
        public virtual Pedido? Pedido { get; set; }
    }
}
