using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Mvc;

namespace Integrador.Controllers
{
    public class MisDatosController : Controller
    {
        private readonly IntegradorContexto _context;

        public MisDatosController(IntegradorContexto context)
        {
            _context = context;
        }

        // GET: MisDatos/CreatePro
        public IActionResult CreatePro()
        {
            return View();
        }

        // POST: MisDatos/CreatePro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePro(
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Proveedor proveedor)
        {
            proveedor.Email = User.Identity.Name;

            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: MisDatos/CreateCli
        public IActionResult CreateCli()
        {
            return View();
        }

        // POST: MisDatos/CreateCli
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCli(
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Cliente cliente)
        {
            cliente.Email = User.Identity.Name;

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Escaparate");
            }

            return View();
        }

    }
}
