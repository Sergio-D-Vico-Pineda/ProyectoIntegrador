using System.ComponentModel.DataAnnotations;

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
        public DateTime? FechaEsperada { get; set; } = DateTime.Now.AddDays(3); // Fecha actual por defecto más 3 días

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
