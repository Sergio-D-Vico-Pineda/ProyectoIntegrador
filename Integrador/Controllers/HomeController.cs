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
            /*string? rolUsuario =  ? "Proveedor" : "Usuario";*/
            string? emailUsuario = User.Identity.Name;

            if (User.IsInRole("Proveedor"))
            {
                var proveedor = _context.Proveedores.FirstOrDefault(p => p.Email == emailUsuario);
                if (proveedor == null)
                {
                    return RedirectToAction("CreatePro", "MisDatos");
                }
            }
            else if (User.IsInRole("Cliente"))
            {
                var cliente = _context.Clientes.FirstOrDefault(c => c.Email == emailUsuario);
                if (cliente == null)
                {
                    return RedirectToAction("CreateCli", "MisDatos");
                }
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
