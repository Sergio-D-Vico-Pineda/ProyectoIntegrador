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
    public class ProveedoresController(IntegradorContexto context, UserManager<IdentityUser> userManager) : Controller
    {
        private readonly IntegradorContexto _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;

        // GET: Proveedores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Proveedores.ToListAsync());
        }

        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores
                .Include(p => p.Suministros)
                    .ThenInclude(s => s.Producto)
                        .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (proveedor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(proveedor);
        }

        // GET: Proveedores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Proveedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Nif,Email,Telefono,Direccion")] Proveedor proveedor)
        {
            if (ProveedorExistsNIF(proveedor.Nif))
            {
                ModelState.AddModelError("Nif", "Ya existe un proveedor con ese NIF.");
            }

            if (UserExists(proveedor.Email) || ProveedorExistsEmail(proveedor.Email))
            {
                ModelState.AddModelError("Email", "Ese email ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.nif2 = proveedor.Nif;

            return View(proveedor);
        }

        // POST: Proveedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string nif2, [Bind("Id,Nombre,Nif,Email,Telefono,Direccion")] Proveedor proveedor)
        {
            if (id != proveedor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (proveedor.Nif != nif2 && ProveedorExistsNIF(proveedor.Nif))
                {
                    ModelState.AddModelError("Nif", "Ya existe un proveedor con ese NIF.");
                    ViewBag.nif2 = nif2;
                    return View(proveedor);
                }

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
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(proveedor);
        }

        // GET: Proveedores/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? admin = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores
                .Include(p => p.Suministros)
                    .ThenInclude(s => s.Producto)
                        .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (proveedor == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.admin = admin;
            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, bool? admin)
        {
            Proveedor? proveedor = await _context.Proveedores
                .Include(p => p.Suministros)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (proveedor == null) return NotFound();

            IdentityUser? user = await _userManager.FindByEmailAsync(proveedor.Email);

            if (proveedor.Suministros.Count == 0)
            // El proveedor no tiene suministros
            {
                // Borro proveedor
                _context.Proveedores.Remove(proveedor);

                // Si el usuario no ha sido eliminado
                if (user != null)
                {
                    // Borro usuario del proveedor
                    IdentityResult? result = await _userManager.DeleteAsync(user);

                    // Manejar el error en caso de que no se pueda eliminar al usuario
                    if (!result.Succeeded) return BadRequest(result.Errors);
                }
            }
            else if (!proveedor.Email.EndsWith("-DEL."))
            // Tiene suministros y El usuario no ha sido eliminado
            {
                // Tacho el proveedor
                proveedor.Email += "-DEL.";
                _context.Update(proveedor);

                // Borro usuario del proveedor
                IdentityResult? result = await _userManager.DeleteAsync(user);

                // Manejar el error en caso de que no se pueda eliminar al usuario
                if (!result.Succeeded) return BadRequest(result.Errors);
            }

            await _context.SaveChangesAsync();

            // Si tiene suministros y ya ha sido borrado no se borra
            if (admin == true) return RedirectToAction(nameof(Index), "Usuarios");
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.Id == id);
        }

        private bool ProveedorExistsEmail(string email)
        {
            return _context.Proveedores.Any(e => e.Email == email);
        }

        private bool ProveedorExistsNIF(string nif)
        {
            return _context.Proveedores.Any(e => e.Nif == nif);
        }

        private bool UserExists(string email)
        {
            var user = _userManager.FindByEmailAsync(email);
            return user.Result != null;
        }
    }
}
