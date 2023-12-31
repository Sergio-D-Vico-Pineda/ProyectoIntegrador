using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Integrador.Controllers
{
    public class MisDatosController : Controller
    {
        private readonly IntegradorContexto _context;

        public MisDatosController(IntegradorContexto context)
        {
            _context = context;
        }

        // PROVEEDORES

        // GET: MisDatos/CreatePro
        [Authorize(Roles = "Proveedor")]
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

        // GET: MisDatos/EditPro
        [Authorize(Roles = "Proveedor")]
        public async Task<IActionResult> EditPro()
        {
            string? email = User.Identity.Name;

            Proveedor? proveedor = await _context.Proveedores
                .Where(p => p.Email == email)
                .FirstOrDefaultAsync();

            if (proveedor == null) return NotFound();

            return View(proveedor);
        }

        // POST: MisDatos/EditPro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPro(int id,
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Proveedor proveedor)
        {
            if (id != proveedor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.Id))
                    {
                        return NotFound();
                    }
                    else
                        throw;

                }

                return RedirectToAction("Index", "Home");
            }

            return View(proveedor);
        }

        private bool ProveedorExists(int id)
        {
            return (_context.Proveedores?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // CLIENTES

        // GET: MisDatos/CreateCli
        [Authorize(Roles = "Cliente")]
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

        // GET: MisDatos/EditCli
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> EditCli()
        {
            string? email = User.Identity.Name;

            Cliente? cliente = await _context.Clientes
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync();

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: MisDatos/EditCli
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCli(int id,
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                        throw;

                }
                return RedirectToAction("Index", "Escaparate");
            }

            return View(cliente);
        }

        private bool ClienteExists(int id)
        {
            return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
