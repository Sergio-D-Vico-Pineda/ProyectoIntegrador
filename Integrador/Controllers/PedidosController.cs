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
    [Authorize(Roles = "Cliente, Administrador")]
    public class PedidosController(IntegradorContexto context) : Controller
    {
        private readonly IntegradorContexto _context = context;

        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            var pedidos = _context.Pedidos
                    .Include(p => p.Cliente)
                    .Include(p => p.Estado)
                    .Include(p => p.DetallePedidos);
            if (User.IsInRole("Administrador"))
            {
                return View(await pedidos
                    .ToListAsync());
            }
            else
            {
                Cliente? cliente = await _context.Clientes
                    .Where(c => c.Email == User.Identity.Name)
                    .FirstOrDefaultAsync();

                if (cliente == null)
                {
                    return RedirectToAction("Create", "MisDatos", new { role = "Cliente" });
                }

                return View(await pedidos
                    .Where(p => p.ClienteId == cliente.Id)
                    .ToListAsync());
            }
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Pedido? pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // GET: Pedidos/Create
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create()
        {
            var listaClientes = await _context.Clientes
                .Where(p => !p.Email.Contains("-DEL."))
                .ToListAsync();

            ViewData["ClienteId"] = new SelectList(listaClientes, "Id", "Email");
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", 1);
            return View();
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaPedido,FechaEsperada,FechaConfirmacion,FechaEnvio,FechaEntrega,FechaAnulado,FechaDevolucion,Comentarios,ClienteId,EstadoId")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Email", pedido.ClienteId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", pedido.EstadoId);
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null) return RedirectToAction(nameof(Index));

            if (User.IsInRole("Cliente"))
            {
                Cliente? cliente = await _context.Clientes
                    .Where(c => c.Email == User.Identity.Name)
                    .FirstOrDefaultAsync();

                if (pedido.ClienteId != cliente.Id)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Email", pedido.ClienteId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", pedido.EstadoId);
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaPedido,FechaEsperada,FechaConfirmacion,FechaEnvio,FechaEntrega,FechaAnulado,FechaDevolucion,Comentarios,ClienteId,EstadoId")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (User.IsInRole("Administrador"))
                    return RedirectToAction(nameof(Index));
                else
                    return RedirectToAction(nameof(Details), new { id = pedido.Id });
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Email", pedido.ClienteId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", pedido.EstadoId);
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Pedido? pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Estado)
                .Include(p => p.DetallePedidos)
                .ThenInclude(dp => dp.Producto)
                .ThenInclude(p => p.Modelo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);

            var detallePedidos = await _context.DetallePedidos
                .Where(dp => dp.PedidoId == id)
                .ToListAsync();

            if (detallePedidos != null)
            {
                foreach (var detalle in detallePedidos)
                {
                    _context.DetallePedidos.Remove(detalle);
                }
            }

            if (pedido != null)
            {
                _context.Pedidos.Remove(pedido);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET /Pedidos/Carrito
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Carrito()
        {
            Cliente? cliente = await _context.Clientes
                    .Where(c => c.Email == User.Identity.Name)
                    .FirstOrDefaultAsync();

            if (User.IsInRole("Cliente") && cliente == null)
            {
                return RedirectToAction("Create", "MisDatos", new { role = "Cliente" });
            }

            var numPed = HttpContext.Session.GetString("NumPedido");
            if (numPed != null)
            {
                return RedirectToAction("Details", "Pedidos", new { id = numPed, c = true });
            }

            return View();
        }

        // PEDIDOS

        // POST /Pedidos/ConfirmarPedido
        [HttpPost, ActionName("Confirmar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPedido(int id)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido != null)
            {
                pedido.EstadoId = 2;
                pedido.FechaConfirmacion = DateTime.Now;
                pedido.FechaEsperada = DateTime.Now.AddDays(7);
                _context.Update(pedido);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("NumPedido");
            }

            return RedirectToAction(nameof(Details), "Pedidos", new { id });
        }

        // POST /Pedidos/Anulado
        [HttpPost, ActionName("Anular")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AnularPedido(int id)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido != null)
            {
                pedido.EstadoId = 5;
                pedido.FechaAnulado = DateTime.Now;
                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), "Pedidos", new { id });
        }

        // POST /Pedidos/Enviado
        [HttpPost, ActionName("Enviado")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviado(int id)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido != null)
            {
                pedido.EstadoId = 3;
                pedido.FechaEnvio = DateTime.Now;
                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), "Pedidos", new { id });
        }

        // POST /Pedidos/Entregado
        [HttpPost, ActionName("Entregado")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Entregado(int id)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido != null)
            {
                pedido.EstadoId = 4;
                pedido.FechaEntrega = DateTime.Now;
                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST /Pedidos/Devolver
        [HttpPost, ActionName("Devolver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Devolver(int id)
        {
            var pedido = await _context.Pedidos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido != null)
            {
                pedido.EstadoId = 6;
                pedido.FechaDevolucion = DateTime.Now;
                _context.Update(pedido);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), "Pedidos", new { id });
        }

        // POST /Pedidos/Mas
        [HttpPost, ActionName("Mas")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasPedido(int id, int dpid)
        {
            DetallePedido? dp = await _context.DetallePedidos
                .Where(dp => dp.PedidoId == id)
                .FirstOrDefaultAsync(dp => dp.Id == dpid);

            dp.Cantidad++;
            _context.Update(dp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), "Pedidos", new { id });
        }

        // POST /Pedidos/Menos
        [HttpPost, ActionName("Menos")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenosPedido(int id, int dpid)
        {
            DetallePedido? dp = await _context.DetallePedidos
                .Where(dp => dp.PedidoId == id)
                .FirstOrDefaultAsync(dp => dp.Id == dpid);

            dp.Cantidad--;
            _context.Update(dp);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), "Pedidos", new { id });
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
