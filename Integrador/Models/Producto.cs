using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class Producto
    {
        [Display(Name = "Id del producto")]
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El nombre es requerido.")]
        public required string Nombre { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es requerida.")]
        public required string Descripcion { get; set; }

        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:n2")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Precio { get; set; }

        [Display(Name = "Precio")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$",
            ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        [Required(ErrorMessage = "El precio es requerido.")]
        public required string PrecioCadena
        {
            get
            {
                return Convert.ToString(Precio).Replace(',', '.');
            }
            set
            {
                Precio = Convert.ToDecimal(value.Replace('.', ','));
            }
        }

        [Required(ErrorMessage = "El campo Escaparate es requerido.")]
        public bool Escaparate { get; set; } // True por defecto

        public string? Imagen { get; set; }

        private int stock; 
        [Range(0, int.MaxValue, ErrorMessage = "El valor debe ser un número entero positivo.")]
        [Required(ErrorMessage = "El stock es requerido.")]
        public int Stock // 0 por defecto
        {
            get => stock;
            set
            {
                if (value >= 0)
                    stock = value;
            }
        }

        [Display(Name = "Modelo")]
        public int ModeloId { get; set; }

        public Modelo? Modelo { get; set; }

        public ICollection<DetallePedido>? DetallePedidos { get; set; }

    }
}
