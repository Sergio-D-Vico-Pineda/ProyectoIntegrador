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
            var integradorContexto = _context.Suministros
                                             .Include(d => d.Proveedor)
                                             .Include(d => d.Producto)
                                             .ThenInclude(d => d.Modelo);
            return View(await integradorContexto.ToListAsync());
        }

        // GET: Suministros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var suministro = await _context.Suministros
                .Include(s => s.Proveedor)
                .Include(s => s.Producto)
                .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (suministro == null)
            {
                return NotFound();
            }

            return View(suministro);
        }

        // GET: Suministros/Create
        public IActionResult Create()
        {
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            ViewData["Modelos"] = new SelectList(_context.Modelos, "Id", "Nombre");
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
                if (suministro.ProductoId == 0)
                {
                    ModelState.AddModelError("ProductoId", "Debe seleccionar un producto.");
                    ViewData["Modelos"] = new SelectList(_context.Modelos, "Id", "Nombre");
                }
                else
                {
                    _context.Add(suministro);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", suministro.ProveedorId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", suministro.ProductoId);

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

            var modeloSeleccionado = await _context.Productos
        .Include(p => p.Modelo)
        .Where(p => p.Id == suministro.ProductoId)
        .Select(p => p.Modelo)
        .FirstOrDefaultAsync();

            // Obtener todos los productos que tengan el mismo modelo que el seleccionado
            var productos = await _context.Productos
                .Where(p => p.ModeloId == modeloSeleccionado.Id)
                .ToListAsync();

            if (suministro == null)
            {
                return NotFound();
            }

            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", suministro.ProveedorId);
            ViewData["ProductoId"] = new SelectList(productos, "Id", "Nombre", suministro.ProductoId);
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
                if (false)
                {
                    ModelState.AddModelError("key", "error");
                }
                else
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
            }
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Nombre", suministro.ProveedorId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", suministro.ProductoId);
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
                .Include(s => s.Proveedor)
                .Include(s => s.Producto)
                .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(s => s.Id == id);
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

        public IActionResult GetProductosByModelo(int? id)
        {
            var productos = _context.Productos.ToList();
            if (id != null)
            {
                productos = _context.Productos.Where(p => p.ModeloId == id).ToList();
            }
            return Json(productos);
        }
        public IActionResult GetModelos()
        {
            var modelos = _context.Modelos.ToList();
            return Json(modelos);
        }
    }
}
