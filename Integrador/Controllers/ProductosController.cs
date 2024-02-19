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
    public class ProductosController(IntegradorContexto context, IWebHostEnvironment webHostEnvironment) : Controller
    {
        private readonly IntegradorContexto _context = context;
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

        // GET: Productos
        [Authorize(Roles = "Proveedor, Administrador")]
        public async Task<IActionResult> Index(string? busq)
        {
            ViewBag.busq = busq;

            var productos = _context.Productos.AsQueryable();

            if (!String.IsNullOrEmpty(busq))
            {
                productos = productos
                    .Where(p => p.Nombre.Contains(busq) || p.Descripcion.Contains(busq) || p.Modelo.Nombre.Contains(busq));
            }

            productos = productos
                .Include(p => p.Modelo)
                .Include(p => p.DetallePedidos);

            return View(await productos.ToListAsync());
        }

        // GET: Productos/Details/5
        [Authorize(Roles = "Proveedor, Cliente, Administrador")]
        public async Task<IActionResult> Details(int? id, int? volver, int? volver2)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Modelo)
                .Include(p => p.DetallePedidos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.volver2 = volver2;
            ViewBag.volver = volver;
            return View(producto);
        }

        // GET: Productos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nombre");
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion,Precio,PrecioCadena,Escaparate,Imagen,Stock,ModeloId")] Producto producto, IFormFile? Imagen)
        {
            if (producto.Precio <= 0)
            {
                ModelState.AddModelError("PrecioCadena", "El precio no puede ser 0 o negativo.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();
                var ultimoProducto = await _context.Productos
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefaultAsync();

                if (Imagen != null)
                {
                    string strRutaImg = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                    string strExt = Path.GetExtension(Imagen.FileName);
                    string strName = ultimoProducto.Id.ToString() + strExt;
                    string strRuta = Path.Combine(strRutaImg, strName);
                    using (var fileStream = new FileStream(strRuta, FileMode.Create, FileAccess.ReadWrite))
                    {
                        Imagen.CopyTo(fileStream);
                    }

                    ultimoProducto.Imagen = strName;
                    try
                    {
                        _context.Update(ultimoProducto);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductoExists(ultimoProducto.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nombre", producto.ModeloId);
            return View(producto);
        }

        // GET: Productos/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);

            if (producto == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nombre", producto.ModeloId);
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion,Precio,PrecioCadena,Escaparate,Imagen,Stock,ModeloId")] Producto producto, IFormFile? Imagen)
        {
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (producto.Precio <= 0)
            {
                ModelState.AddModelError("PrecioCadena", "El precio no puede ser 0 o negativo.");
            }

            if (ModelState.IsValid)
            {

                if (Imagen != null)
                {
                    string strRutaImg = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                    string strExt = Path.GetExtension(Imagen.FileName);
                    string strName = producto.Id.ToString() + strExt;
                    string strRuta = Path.Combine(strRutaImg, strName);
                    using (var fileStream = new FileStream(strRuta, FileMode.Create, FileAccess.ReadWrite))
                    {
                        Imagen.CopyTo(fileStream);
                    }

                    producto.Imagen = strName;
                }

                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.Id))
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
            ViewData["ModeloId"] = new SelectList(_context.Modelos, "Id", "Nombre", producto.ModeloId);
            return View(producto);
        }

        // GET: Productos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id, int? volver)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Modelo)
                .Include(p => p.DetallePedidos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.volver = volver;
            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? volver)
        {
            Producto? producto = await _context.Productos.FindAsync(id);

            if (producto == null) return NotFound();

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            if (producto.Imagen != null)
            {
                string strRutaImg = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                string strRuta = Path.Combine(strRutaImg, producto.Imagen);
                System.IO.File.Delete(strRuta);
            }

            if (volver == null)
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction(nameof(Delete), "Modelos", new { id = volver });
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
