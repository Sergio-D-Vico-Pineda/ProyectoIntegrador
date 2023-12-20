using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class Producto
    {
        [Display(Name = "Id del producto")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido.")]
        public required string Nombre { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es requerida.")]
        public required string Descripcion { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2")]
        [Column(TypeName = "decimal(18, 2)")]
        [Required(ErrorMessage = "El precio es requerido.")]
        public required decimal Precio { get; set; }

        [Display(Name = "Precio")]
        [RegularExpression(@"^[-0123456789]+[0-9.,]*$",
            ErrorMessage = "El valor introducido debe ser de tipo monetario.")]
        [Required(ErrorMessage = "El precio cadena es requerido.")]
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
        public required bool Escaparate { get; set; } = true;

        public string? Imagen { get; set; }

        public required int Stock { get; set; } = 0;

        public int ModeloId { get; set; }

        public Modelo? Modelo { get; set; }

        public ICollection<DetallePedido>? DetallePedidos { get; set; }

    }
}
