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

        [Display(Name = "Precio Unitario")]
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "El precio de unidad es requerido.")]
        public required decimal PrecioUnidad { get; set; }

        [Display(Name = "Precio Total")]
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "El precio total es requerido.")]
        public required decimal PrecioTotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Descuento { get; set; }

        public virtual Producto? Producto { get; set; }
        public virtual Pedido? Pedido { get; set; }
    }
}
