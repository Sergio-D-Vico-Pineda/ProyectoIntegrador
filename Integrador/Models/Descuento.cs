using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Integrador.Models
{
    public class Descuento
    {
        public int Id { get; set; }

        private string codigo = "";

        [MaxLength(6)]
        [Display(Name = "Código")]
        [Required(ErrorMessage = "Se necesita un código.")]
        public required string Codigo
        {
            get => codigo;
            set => codigo = value == null ? "" : value.ToUpper();
        }

        public decimal? porcentaje;

        [Display(Name = "Porcentaje")]
        [Required(ErrorMessage = "Se necesita un porcentaje.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?%?$|^\d+,\d{1,2}%?$", ErrorMessage = "El valor introducido debe ser un porcentaje válido.")]
        public required string? Porcentaje
        {
            get => $"{porcentaje:0.00}".Replace(',', '.');
            set => porcentaje = decimal.TryParse(value.Replace('.', ','), out var result) ? result : 0;
        }
    }
}
