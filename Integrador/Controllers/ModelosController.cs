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

namespace Integrador.Controllers
{
    public class ModelosController(IntegradorContexto context) : Controller
    {
        private readonly IntegradorContexto _context = context;

        // GET: Modelos
        [Authorize(Roles = "Proveedor, Administrador")]
        public async Task<IActionResult> Index()
        {
            var modelos = _context.Modelos
                .Include(m => m.Marca)
                .Include(m => m.Productos);
            return View(await modelos.ToListAsync());
        }

        // GET: Modelos/Details/5
        [Authorize(Roles = "Proveedor, Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelo = await _context.Modelos
                .Include(m => m.Marca)
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modelo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(modelo);
        }

        // GET: Modelos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre");
            return View();
        }

        // POST: Modelos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,MarcaId")] Modelo modelo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(modelo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", modelo.MarcaId);
            return View(modelo);
        }

        // GET: Modelos/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelo = await _context.Modelos.FindAsync(id);

            if (modelo == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", modelo.MarcaId);
            return View(modelo);
        }

        // POST: Modelos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,MarcaId")] Modelo modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(modelo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ModeloExists(modelo.Id))
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
            ViewData["MarcaId"] = new SelectList(_context.Marcas, "Id", "Nombre", modelo.MarcaId);
            return View(modelo);
        }

        // GET: Modelos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var modelo = await _context.Modelos
                .Include(m => m.Marca)
                .Include(m => m.Productos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (modelo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(modelo);
        }

        // POST: Modelos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var modelo = await _context.Modelos.FindAsync(id);
            if (modelo != null)
            {
                _context.Modelos.Remove(modelo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ModeloExists(int id)
        {
            return _context.Modelos.Any(e => e.Id == id);
        }
    }
}
