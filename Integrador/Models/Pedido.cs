using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class Pedido
    {
        [Display(Name = "Número de Pedido")]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de pedido")]
        public DateTime? FechaPedido { get; set; } = DateTime.Now; // Fecha actual por defecto

        [DataType(DataType.Date)]
        [Display(Name = "Fecha estimada")]
        public DateTime? FechaEsperada { get; set; } 

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de confirmación")]
        public DateTime? FechaConfirmacion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de envio")]
        public DateTime? FechaEnvio { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de entrega")]
        public DateTime? FechaEntrega { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de anulación")]
        public DateTime? FechaAnulado { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de devolución")]
        public DateTime? FechaDevolucion { get; set; }

        public string? Comentarios { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? Descuento { get; set; }

        [StringLength(6)]
        [Display(Name = "Código de descuento")]
        public string? CodDescuento { get; set; }

        [Display(Name = "Cliente")]
        [Required(ErrorMessage = "El cliente es obligatorio.")]
        public required int ClienteId { get; set; }

        [Display(Name = "Estado")]
        [Required(ErrorMessage = "El estado es obligatorio.")]
        public required int EstadoId { get; set; }

        public Cliente? Cliente { get; set; }
        public Estado? Estado { get; set; }

        public ICollection<DetallePedido>? DetallePedidos { get; set; }
    }
}
