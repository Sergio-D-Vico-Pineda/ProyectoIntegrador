﻿using System;
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
            if (ClienteExistsNif(cliente.Nif))
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
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string nif2, [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return NotFound();
            }

            if (cliente.Nif != nif2)
            {
                if (ClienteExistsNif(nif2))
                {
                    ModelState.AddModelError("Nif", "Ya existe un cliente con ese NIF.");
                    ViewBag.nif2 = nif2;
                }
                else cliente.Nif = nif2;
            }

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
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
            }

            await _context.SaveChangesAsync();

            if (admin == true && User.IsInRole("Administrador"))
            {
                var user = await _userManager.FindByEmailAsync(cliente.Email);
                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    // Manejar el error en caso de que no se pueda eliminar al usuario
                    return BadRequest(result.Errors);
                }

                return RedirectToAction(nameof(Index), "Usuarios");
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }

        private bool ClienteExistsNif(string nif)
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
