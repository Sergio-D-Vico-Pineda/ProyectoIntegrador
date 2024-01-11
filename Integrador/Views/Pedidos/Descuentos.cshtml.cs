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
        public ModelDescuentos Descuento { get; set; }

        public class ModelDescuentos
        {
            [StringLength(6)]
            public string? Codigo { get; set; }
        }
    }
}
