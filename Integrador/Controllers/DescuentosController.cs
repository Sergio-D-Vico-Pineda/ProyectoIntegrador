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
    [Authorize(Roles = "Administrador")]
    public class DescuentosController(IntegradorContexto context) : Controller
    {
        private readonly IntegradorContexto _context = context;

        // GET: Descuentos
        public async Task<IActionResult> Index()
        {
            return View(await _context.Descuentos.ToListAsync());
        }

        // GET: Descuentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var descuento = await _context.Descuentos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (descuento == null)
            {
                return NotFound();
            }

            return View(descuento);
        }

        // GET: Descuentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Descuentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Codigo,Porcentaje")] Descuento descuento)
        {
            if (_context.Descuentos.Any(x => x.Codigo == descuento.Codigo))
            {
                ModelState.AddModelError("Codigo", "Ya existe un descuento con ese código");
            }

            if (descuento.porcentaje <= 0 || descuento.porcentaje >= 100)
            {
                ModelState.AddModelError("Porcentaje", "El porcentaje debe estar entre 0 y 100");
            }

            if (ModelState.IsValid)
            {
                _context.Add(descuento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(descuento);
        }

        // GET: Descuentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var descuento = await _context.Descuentos.FindAsync(id);
            if (descuento == null)
            {
                return NotFound();
            }
            return View(descuento);
        }

        // POST: Descuentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Porcentaje")] Descuento descuento)
        {
            if (id != descuento.Id)
            {
                return NotFound();
            }

            if (descuento.porcentaje <= 0 || descuento.porcentaje >= 100)
            {
                ModelState.AddModelError("Porcentaje", "El porcentaje debe estar entre 0 y 100.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(descuento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DescuentoExists(descuento.Id))
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
            return View(descuento);
        }

        // GET: Descuentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var descuento = await _context.Descuentos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (descuento == null)
            {
                return NotFound();
            }

            return View(descuento);
        }

        // POST: Descuentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var descuento = await _context.Descuentos.FindAsync(id);
            if (descuento != null)
            {
                _context.Descuentos.Remove(descuento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DescuentoExists(int id)
        {
            return _context.Descuentos.Any(e => e.Id == id);
        }
    }
}
