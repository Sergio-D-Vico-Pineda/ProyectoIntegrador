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
    
    public class DetallePedidosController(IntegradorContexto context) : Controller
    {
        private readonly IntegradorContexto _context = context;

        // GET: DetallePedidos
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var detallePedidos = _context.DetallePedidos
                            .Include(dp => dp.Pedido)
                            .Include(dp => dp.Producto)
                            .ThenInclude(p => p.Modelo);
            return View(await detallePedidos.ToListAsync());
        }

        // GET: DetallePedidos/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallePedidos
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (detallePedido == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(detallePedido);
        }

        // GET: DetallePedidos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id");
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre");
            return View();
        }

        // POST: DetallePedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PedidoId,ProductoId,Cantidad,PrecioUnidad,PrecioTotal,Descuento")] DetallePedido detallePedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallePedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedido.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedido.ProductoId);
            return View(detallePedido);
        }

        // GET: DetallePedidos/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallePedidos.FindAsync(id);

            if (detallePedido == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedido.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedido.ProductoId);
            return View(detallePedido);
        }

        // POST: DetallePedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PedidoId,ProductoId,Cantidad,PrecioUnidad,PrecioTotal,Descuento")] DetallePedido detallePedido)
        {
            if (id != detallePedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallePedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallePedidoExists(detallePedido.Id))
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
            ViewData["PedidoId"] = new SelectList(_context.Pedidos, "Id", "Id", detallePedido.PedidoId);
            ViewData["ProductoId"] = new SelectList(_context.Productos, "Id", "Nombre", detallePedido.ProductoId);
            return View(detallePedido);
        }

        // GET: DetallePedidos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallePedido = await _context.DetallePedidos
                .Include(d => d.Pedido)
                .Include(d => d.Producto)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (detallePedido == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(detallePedido);
        }

        // POST: DetallePedidos/Delete/5
        [Authorize(Roles = "Cliente, Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? pedidoid)
        {
            var detallePedido = await _context.DetallePedidos.FindAsync(id);
            if (detallePedido != null)
            {
                _context.DetallePedidos.Remove(detallePedido);
            }

            await _context.SaveChangesAsync();

            if (pedidoid != null)
            {
                return RedirectToAction(nameof(Details), "Pedidos", new { id = pedidoid });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool DetallePedidoExists(int id)
        {
            return _context.DetallePedidos.Any(e => e.Id == id);
        }
    }
}
