using System.ComponentModel.DataAnnotations;

namespace Integrador.Models
{
    public class Estado
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es necesario.")]
        public required string Nombre { get; set; }

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es requerida.")]
        public required string Descripcion { get; set; }

        public ICollection<Pedido>? Pedidos { get; set; }
    }
}
