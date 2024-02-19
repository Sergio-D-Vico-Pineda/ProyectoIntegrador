using System.ComponentModel.DataAnnotations;

namespace Integrador.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public required string Nombre { get; set; }

        [StringLength(50)]
        [Display(Name = "Correo electrónico")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido.")]
        public required string Email { get; set; }

        [StringLength(10)]
        [Display(Name = "NIF")]
        [Required(ErrorMessage = "El NIF es obligatorio.")]
        /*[RegularExpression(@"^\d{8}[A-Za-z]$", ErrorMessage = "El NIF no es válido.")]*/
        public required string Nif { get; set; }

        [StringLength(12)]
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public required string Telefono { get; set; }

        [Display(Name = "Dirección")]
        [Required(ErrorMessage = "La dirección es obligatoria.")]
        public required string Direccion { get; set; }

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
