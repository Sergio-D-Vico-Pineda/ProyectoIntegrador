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

namespace Integrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstadosController(IntegradorContexto context) : Controller
    {
        private readonly IntegradorContexto _context = context;

        // GET: Estados
        public async Task<IActionResult> Index()
        {
            var estados = _context.Estados
                .Include(e => e.Pedidos);

            return View(await estados.ToListAsync());
        }

        // GET: Estados/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Estado estado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(estado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // GET: Estados/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = await _context.Estados.FindAsync(id);

            if (estado == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(estado);
        }

        // POST: Estados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Estado estado)
        {
            if (id != estado.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(estado);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EstadoExists(estado.Id))
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
            return View(estado);
        }

        // GET: Estados/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estado = await _context.Estados
                .FirstOrDefaultAsync(m => m.Id == id);

            if (estado == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(estado);
        }

        // POST: Estados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estado = await _context.Estados.FindAsync(id);
            if (estado != null)
            {
                _context.Estados.Remove(estado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EstadoExists(int id)
        {
            return _context.Estados.Any(e => e.Id == id);
        }
    }
}
