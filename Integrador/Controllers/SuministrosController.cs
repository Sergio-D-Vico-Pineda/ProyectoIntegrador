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
    [Authorize(Roles = "Proveedor, Administrador")]
    public class SuministrosController(IntegradorContexto context) : Controller
    {
        private readonly IntegradorContexto _context = context;

        // GET: Suministros
        public async Task<IActionResult> Index(string? busq)
        {
            var suministros = _context.Suministros
                    .Include(d => d.Proveedor)
                    .Include(d => d.Producto)
                    .ThenInclude(p => p.Modelo)
                    .OrderByDescending(s => s.Id);

            ViewBag.busq = busq;

            if (!String.IsNullOrEmpty(busq))
            {
                suministros = (IOrderedQueryable<Suministro>)suministros
                    .Where(s => s.Producto.Nombre.Contains(busq) || s.Producto.Modelo.Nombre.Contains(busq));
            }

            if (User.IsInRole("Administrador"))
            {
                return View(await suministros.ToListAsync());
            }
            else
            {
                Proveedor? proveedor = await _context.Proveedores
                                .Where(p => p.Email == User.Identity.Name)
                                .FirstOrDefaultAsync();

                if (proveedor == null) return RedirectToAction("Create", "MisDatos", new { role = "Proveedor" });

                return View(await suministros
                        .Where(s => s.ProveedorId == proveedor.Id)
                        .ToListAsync());
            }
        }

        // GET: Suministros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Suministro? suministro = await _context.Suministros
                .Include(s => s.Proveedor)
                .Include(s => s.Producto)
                .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (suministro == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(suministro);
        }

        // GET: Suministros/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (User.IsInRole("Proveedor"))
            {
                // Obtener el proveedor actual
                Proveedor? proveedor = await _context.Proveedores.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);

                // Si el cliente no existe, redirigir a la vista de creación de cuenta
                if (proveedor == null) return RedirectToAction(nameof(Create), "MisDatos", new { role = "Proveedor" });
            }

            if (User.IsInRole("Administrador"))
            {
                var listaProveedores = await _context.Proveedores
                    .Where(p => !p.Email.EndsWith("-DEL."))
                    .ToListAsync();

                ViewData["ProveedorId"] = new SelectList(listaProveedores, "Id", "Email");
            }

            if (id != null)
            {
                var proveedor = _context.Proveedores
                    .Where(p => p.Email == User.Identity.Name)
                    .FirstOrDefault();
                if (proveedor != null)
                    ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Email", proveedor.Id);
                ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", id);
            }

            ViewData["Modelos"] = new SelectList(_context.Modelos.OrderBy(m => m.Nombre), "Id", "Nombre");

            return View();
        }

        // POST: Suministros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProveedorId,ProductoId,Unidades,FechaSuministro")] Suministro suministro)
        {
            if (User.IsInRole("Proveedor"))
            {
                // Asignar proveedor al suministro
                var proveedorId = await _context.Proveedores
                            .Where(p => p.Email == User.Identity.Name)
                            .FirstOrDefaultAsync();
                if (proveedorId != null)
                {
                    suministro.ProveedorId = proveedorId.Id;
                }
            }

            if (ModelState.IsValid)
            {
                if (suministro.ProductoId == 0)
                {
                    ModelState.AddModelError("ProductoId", "Debe seleccionar un producto.");
                }
                else
                {
                    var producto = await _context.Productos.FindAsync(suministro.ProductoId);
                    producto.Stock += suministro.Unidades;

                    _context.Add(suministro);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Email", suministro.ProveedorId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", suministro.ProductoId);
            ViewData["Modelos"] = new SelectList(_context.Modelos, "Id", "Nombre");

            return View(suministro);
        }

        // GET: Suministros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var suministro = await _context.Suministros.FindAsync(id);

            if (id == null) return NotFound();

            if (suministro == null) return RedirectToAction(nameof(Index));

            if (User.IsInRole("Proveedor"))
            {
                // Comprobar que el suministro pertence al proveedor
                Proveedor? proveedor = await _context.Proveedores
                            .Where(p => p.Email == User.Identity.Name)
                            .FirstOrDefaultAsync();

                if (suministro.ProveedorId != proveedor.Id)
                {
                    return RedirectToAction(nameof(Index));
                }

            }

            var modeloSeleccionado = await _context.Productos
                    .Include(p => p.Modelo)
                    .Where(p => p.Id == suministro.ProductoId)
                    .Select(p => p.Modelo)
                    .FirstOrDefaultAsync();

            // Obtener todos los productos que tengan el mismo modelo que el seleccionado
            var productos = await _context.Productos
                .Where(p => p.ModeloId == modeloSeleccionado.Id)
                .ToListAsync();

            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Email", suministro.ProveedorId);
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

            if (User.IsInRole("Proveedor"))
            {
                // Asignar proveedor al suministro
                var proveedor = await _context.Proveedores
                            .Where(p => p.Email == User.Identity.Name)
                            .FirstOrDefaultAsync();
                if (proveedor != null)
                {
                    suministro.ProveedorId = proveedor.Id;
                }
            }

            if (ModelState.IsValid)
            {
                var suministroOriginal = await _context.Suministros.AsNoTracking().FirstOrDefaultAsync(s => s.Id == suministro.Id);
                var diferenciaUnidades = suministro.Unidades - suministroOriginal.Unidades;

                var producto = await _context.Productos.FindAsync(suministro.ProductoId);
                producto.Stock += diferenciaUnidades;

                // Verificar si el stock es menor que 0 y establecerlo en 0 si es el caso
                if (producto.Stock < 0)
                {
                    producto.Stock = 0;
                }
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
            ViewData["ProveedorId"] = new SelectList(_context.Proveedores, "Id", "Email", suministro.ProveedorId);
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
                return RedirectToAction(nameof(Index));
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

                var producto = await _context.Productos.FindAsync(suministro.ProductoId);
                producto.Stock -= suministro.Unidades;

                // Verificar si el stock es menor que 0 y establecerlo en 0 si es el caso
                if (producto.Stock < 0)
                {
                    producto.Stock = 0;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SuministroExists(int id)
        {
            return _context.Suministros.Any(e => e.Id == id);
        }

        public async Task<IActionResult> GetProductosByModelo(int? id)
        {
            var productos = _context.Productos;
            if (id != null)
            {
                return Json(await productos.Where(p => p.ModeloId == id).ToListAsync());
            }
            return Json(await productos.ToListAsync());
        }

        public async Task<IActionResult> GetModeloByProducto(int? id)
        {
            var modeloSeleccionado = await _context.Productos
                .Include(p => p.Modelo)
                .Where(p => p.Id == id)
                .Select(p => p.Modelo)
                .FirstOrDefaultAsync();

            return Json(modeloSeleccionado);
        }

    }
}
