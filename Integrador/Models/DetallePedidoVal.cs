using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class DetallePedido1
    {
        private decimal _precioTotal;
        private decimal _precioUnidad;

        public int Id { get; set; }

        [Display(Name = "Número de pedido")]
        public int PedidoId { get; set; }

        [Display(Name = "Id del producto")]
        public int ProductoId { get; set; }

        public int Cantidad { get; set; }

        [Display(Name = "Precio Unitario")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioUnidad
        {
            get => _precioUnidad;
            set
            {
                _precioUnidad = value;
                _precioTotal = _precioUnidad * Cantidad;
            }
        }

        [Display(Name = "Precio Total")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecioTotal
        {
            get { return _precioTotal; }
            set { _precioTotal = _precioUnidad * Cantidad; }
        }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Descuento { get; set; }

        public Producto? Producto { get; set; }

        public Pedido? Pedido { get; set; }
    }
}
