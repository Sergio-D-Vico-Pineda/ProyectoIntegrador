using System.ComponentModel.DataAnnotations;

namespace Integrador.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [Display(Name = "Correo electrónico")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El NIF es obligatorio.")]
        [Display(Name = "NIF")]
        public required string Nif { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Display(Name = "Teléfono")]
        public required string Telefono { get; set; }

        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
