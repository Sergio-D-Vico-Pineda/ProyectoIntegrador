using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Integrador.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IntegradorContexto _context;

        public HomeController(ILogger<HomeController> logger, IntegradorContexto context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            string? emailUsuario = User.Identity.Name;

            if (User.IsInRole("Proveedor") && !_context.Proveedores.Any(p => p.Email == emailUsuario))
            {
                return RedirectToAction("Create", "MisDatos", new { role = "Proveedor" });
            }

            if (User.IsInRole("Cliente") && !_context.Clientes.Any(c => c.Email == emailUsuario))
            {
                return RedirectToAction("Create", "MisDatos", new { role = "Cliente" });
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
