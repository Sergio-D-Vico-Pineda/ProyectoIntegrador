using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Integrador.Views.Pedidos
{
    public class Descuentos
    {
        public Descuentos()
        {
        }

        [BindProperty]
        [StringLength(6)]
        public string? Codigo { get; set; }

    }
}
