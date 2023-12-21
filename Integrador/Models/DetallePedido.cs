using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Display(Name = "Número de pedido")]
        public int PedidoId { get; set; }

        [Display(Name = "Id del producto")]
        public int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        public required int Cantidad { get; set; }

        private decimal? precioUnidad;

        [Display(Name = "Precio Unitario")]
        [Range(0.1, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PrecioUnidad // Mayor que 0
        {
            get => precioUnidad;
            set
            {
                if (value > 0)
                    precioUnidad = value;
            }
        }

        [Display(Name = "Precio Total")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PrecioTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Descuento { get; set; }

        public virtual Producto? Producto { get; set; }
        public virtual Pedido? Pedido { get; set; }
    }
}
