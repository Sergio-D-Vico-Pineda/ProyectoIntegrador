using Integrador.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Integrador.Controllers
{
    public class EscaparateController : Controller
    {

        private readonly IntegradorContexto _context;

        public EscaparateController(IntegradorContexto context)
        {
            _context = context;
        }

        // GET /Escaparate/Index
        public async Task<IActionResult> Index(int? MarcaId)
        {
            ViewData["ListaMarcas"] = new SelectList(_context.Marcas, "Id", "Nombre");
            var productos = _context.Productos
                .Include(p => p.Modelo)
                .Where(p => p.Escaparate);
            return View(await productos.ToListAsync());
        }
    }
}
