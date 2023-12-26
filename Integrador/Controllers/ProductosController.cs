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
    public class ProductosController : Controller
    {
        private readonly IntegradorContexto _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductosController(IntegradorContexto context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var integradorContexto = _context.Productos.Include(p => p.Modelo);
            return View(await integradorContexto.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Modelo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Modelo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();

            if (producto.Imagen != null)
            {
                string strRutaImg = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                string strRuta = Path.Combine(strRutaImg, producto.Imagen);
                System.IO.File.Delete(strRuta);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.Id == id);
        }
    }
}
