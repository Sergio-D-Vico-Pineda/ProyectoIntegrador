using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Integrador.Data;
using Integrador.Models;

namespace Integrador.Controllers
{
    public class SuministrosController : Controller
    {
        private readonly IntegradorContexto _context;

        public SuministrosController(IntegradorContexto context)
        {
            _context = context;
        }

        // GET: Suministros
        public async Task<IActionResult> Index()
        {
            return View(await _context.Suministros.ToListAsync());
        }

        // GET: Suministros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suministro = await _context.Suministros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suministro == null)
            {
                return NotFound();
            }

            return View(suministro);
        }

        // GET: Suministros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Suministros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProveedorId,ProductoId,Unidades,FechaSuministro")] Suministro suministro)
        {
            if (ModelState.IsValid)
            {
                _context.Add(suministro);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(suministro);
        }

        // GET: Suministros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suministro = await _context.Suministros.FindAsync(id);
            if (suministro == null)
            {
                return NotFound();
            }
            return View(suministro);
        }

        // POST: Suministros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProveedorId,ProductoId,Unidades,FechaSuministro")] Suministro suministro)
        {
            if (id != suministro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(suministro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SuministroExists(suministro.Id))
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
            return View(suministro);
        }

        // GET: Suministros/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suministro = await _context.Suministros
                .FirstOrDefaultAsync(m => m.Id == id);
            if (suministro == null)
            {
                return NotFound();
            }

            return View(suministro);
        }

        // POST: Suministros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var suministro = await _context.Suministros.FindAsync(id);
            if (suministro != null)
            {
                _context.Suministros.Remove(suministro);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuministroExists(int id)
        {
            return _context.Suministros.Any(e => e.Id == id);
        }
    }
}
