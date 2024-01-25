using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class DetallePedido
    {
        public int Id { get; set; }

        [Display(Name = "Número de pedido")]
        public required int PedidoId { get; set; }

        [Display(Name = "Id del producto")]
        public required int ProductoId { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida.")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Debe contener solo números enteros positivos.")]
        [Range(1, double.MaxValue, ErrorMessage = "La cantidad tiene que ser mayor que 0.")]
        public required int Cantidad { get; set; }

        [Display(Name = "Precio")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnidad { get; set; }

        [Display(Name = "Precio Unitario")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$",
            ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        [Required(ErrorMessage = "El precio es requerido.")]
        public string? PrecioUnidadCadena
        {
            get
            {
                return Convert.ToString(PrecioUnidad).Replace(',', '.');
            }
            set
            {
                string a = value.ToString();
                if ( a == "")
                {
                    a = "0";
                }
                PrecioUnidad = Convert.ToDecimal(a.Replace('.', ','));
            }
        }

        public virtual Producto? Producto { get; set; }
        public virtual Pedido? Pedido { get; set; }
    }
}
