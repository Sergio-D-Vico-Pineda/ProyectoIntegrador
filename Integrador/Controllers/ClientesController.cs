using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Integrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ClientesController(IntegradorContexto context, UserManager<IdentityUser> userManager) : Controller
    {
        private readonly IntegradorContexto _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Clientes.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                .ThenInclude(p => p.Estado)
                .Include(c => c.Pedidos)
                .ThenInclude(p => p.DetallePedidos)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Cliente cliente)
        {
            if (ClienteExistsNIF(cliente.Nif))
            {
                ModelState.AddModelError("Nif", "Ya existe un cliente con ese NIF.");
            }

            if (UserExists(cliente.Email) || ClienteExistsEmail(cliente.Email))
            {
                ModelState.AddModelError("Email", "Ese email ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.nif2 = cliente.Nif;

            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string nif2, [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (cliente.Nif != nif2 && ClienteExistsNIF(cliente.Nif))
                {
                    ModelState.AddModelError("Nif", "Ya existe un cliente con ese NIF.");
                    ViewBag.nif2 = nif2;
                    return View(cliente);
                }

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
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? admin = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                .ThenInclude(p => p.Estado)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (cliente == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.admin = admin;
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool? admin)
        {
            Cliente? cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (cliente == null) return NotFound();

            IdentityUser? user = await _userManager.FindByEmailAsync(cliente.Email);

            if (cliente.Pedidos.Count == 0)
            // El cliente no tiene pedidos
            {
                // Borro cliente
                _context.Clientes.Remove(cliente);

                // Si el usuario no ha sido eliminado
                if (user != null)
                {
                    // Borro usuario del cliente
                    IdentityResult? result = await _userManager.DeleteAsync(user);

                    // Manejar el error en caso de que no se pueda eliminar al usuario
                    if (!result.Succeeded) return BadRequest(result.Errors);
                }
            }
            else if (!cliente.Email.EndsWith("-DEL."))
            // Tiene pedidos y El usuario no ha sido eliminado
            {
                // Tacho el cliente
                cliente.Email += "-DEL.";
                _context.Update(cliente);

                // Borro usuario del cliente
                IdentityResult? result = await _userManager.DeleteAsync(user);

                // Manejar el error en caso de que no se pueda eliminar al usuario
                if (!result.Succeeded) return BadRequest(result.Errors);
            }

            await _context.SaveChangesAsync();

            // Si tiene pedidos y ya ha sido borrado no se borra
            if (admin == true) return RedirectToAction(nameof(Index), "Usuarios");
            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        private bool ClienteExistsNIF(string nif)
        {
            return _context.Clientes.Any(e => e.Nif == nif);
        }

        private bool ClienteExistsEmail(string email)
        {
            return _context.Clientes.Any(e => e.Email == email);
        }

        private bool UserExists(string email)
        {
            var user = _userManager.FindByEmailAsync(email);
            return user.Result != null;
        }
    }
}
