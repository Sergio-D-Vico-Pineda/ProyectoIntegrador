﻿using System.ComponentModel.DataAnnotations;

namespace Integrador.Models
{
    public class Pedido
    {
        [Display(Name = "Número de Pedido")]
        public int Id { get; set; }

        [Display(Name = "Fecha de pedido")]
        [DataType(DataType.Date)]
        public DateTime? FechaPedido { get; set; } = DateTime.Now; // Fecha actual por defecto

        [Display(Name = "Fecha esperada de entrega")]
        [DataType(DataType.Date)]
        public DateTime? FechaEsperada { get; set; } = DateTime.Now.AddDays(3); // Fecha actual por defecto más 3 días

        [Display(Name = "Fecha de confirmación")]
        [DataType(DataType.Date)]
        public DateTime? FechaConfirmacion { get; set; }

        [Display(Name = "Fecha de envio")]
        [DataType(DataType.Date)]
        public DateTime? FechaEnvio { get; set; }

        [Display(Name = "Fecha de entrega")]
        [DataType(DataType.Date)]
        public DateTime? FechaEntrega { get; set; }

        [Display(Name = "Fecha de anulación")]
        [DataType(DataType.Date)]
        public DateTime? FechaAnulado { get; set; }

        [Display(Name = "Fecha de devolución")]
        [DataType(DataType.Date)]
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
