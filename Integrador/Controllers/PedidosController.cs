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
using Integrador.Views.Pedidos;

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
                    .OrderByDescending(p => p.Id)
                    .ToListAsync());
            }
        }

        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id, bool? c)
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

            if (User.IsInRole("Cliente"))
            {
                Cliente? cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);
                if (pedido.ClienteId != cliente.Id)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            if (pedido == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.carrito = c ?? false;

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
        public async Task<IActionResult> Create([Bind("Id,FechaPedido,FechaEsperada,FechaConfirmacion,FechaEnvio,FechaEntrega,FechaAnulado,FechaDevolucion,Comentarios,Descuento,ClienteId,EstadoId")] Pedido pedido)
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaPedido,FechaEsperada,FechaConfirmacion,FechaEnvio,FechaEntrega,FechaAnulado,FechaDevolucion,Comentarios,Descuento,ClienteId,EstadoId")] Pedido pedido)
        {
            if (id != pedido.Id) return NotFound();

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
            Cliente? cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);

            if (User.IsInRole("Cliente") && cliente == null)
            {
                return RedirectToAction("Create", "MisDatos", new { role = "Cliente" });
            }

            var listaPedidos = await _context.Pedidos
                    .Where(p => p.EstadoId == 1)
                    .Where(p => p.ClienteId == cliente.Id)
                    .ToListAsync();

            if (listaPedidos.Count > 0)
            {
                var ultimo = listaPedidos
                    .OrderByDescending(p => p.Id)
                    .First(); // Obtener el último pedido pendiente

                if (ultimo != null)
                    HttpContext.Session.SetString("NumPedido", ultimo.Id.ToString());
            }

            var numPed = HttpContext.Session.GetString("NumPedido");
            if (numPed != null)
            {
                return RedirectToAction("Details", "Pedidos", new { id = numPed, c = true });
            }

            return View();
        }

        // GET /Pedidos/Descuentos
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Descuentos(int? id)
        {
            if (id == null) return NotFound();

            var pedido = await _context.Pedidos.FindAsync(id);

            if (pedido == null) return RedirectToAction(nameof(Index));

            Cliente? cliente = await _context.Clientes
                .Where(c => c.Email == User.Identity.Name)
                .FirstOrDefaultAsync();

            if (pedido.ClienteId != cliente.Id) return RedirectToAction(nameof(Index));

            ViewBag.id = id;
            return View();
        }

        // POST /Pedidos/Descuentos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Descuentos(int id, [Bind("Codigo")] Descuentos desc)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido == null) return NotFound();

            string codigo = desc.Codigo ?? "";

            await CalcDescuentos(id, codigo);

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
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Email", pedido.ClienteId);
            ViewData["EstadoId"] = new SelectList(_context.Estados, "Id", "Descripcion", pedido.EstadoId);

            return RedirectToAction(nameof(Carrito));
        }

        private async Task CalcDescuentos(int pid, string codigo)
        {
            decimal? total = 0;
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .FirstOrDefaultAsync(m => m.Id == pid);

            foreach (var dp in pedido.DetallePedidos)
            {
                total += dp.Cantidad * dp.PrecioUnidad;
            }

            switch (codigo.ToUpper())
            {
                case "SCARPY":
                    {
                        pedido.CodDescuento = "SCARPY";
                        pedido.Descuento = total * 0.9m;
                    }
                    break;
                case "DWES":
                    {
                        pedido.CodDescuento = "DWES";
                        pedido.Descuento = total * 0.1m;
                    }
                    break;
                case "DAW":
                    {
                        pedido.CodDescuento = "DAW";
                        pedido.Descuento = total * 0.8m;
                    }
                    break;
                case "RRBOX":
                    {
                        pedido.CodDescuento = "RRBOX";
                        pedido.Descuento = total * 0.2m;
                    }
                    break;
                case "SEXO":
                    {
                        pedido.CodDescuento = "SEXO";
                        pedido.Descuento = total * 0.5m;
                    }
                    break;
                default:
                    {
                        pedido.CodDescuento = null;
                        pedido.Descuento = 0;
                    }
                    break;
            }
        }

        // PEDIDOS

        // POST /Pedidos/ConfirmarPedido
        [HttpPost, ActionName("Confirmar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido != null)
            {
                pedido.EstadoId = 2;
                pedido.FechaConfirmacion = DateTime.Now;
                pedido.FechaEsperada = DateTime.Now.AddDays(7);
                _context.Update(pedido);

                foreach (var dp in pedido.DetallePedidos)
                {
                    // Restar stock en los productos
                    var producto = await _context.Productos.FindAsync(dp.ProductoId);

                    if (producto == null) continue;

                    // Comprobar que hay stock en el producto
                    if (producto.Stock >= dp.Cantidad && dp.Cantidad > 0)
                    {
                        producto.Stock -= dp.Cantidad;
                        _context.Update(producto);
                    }
                    else
                    {
                        return RedirectToAction(nameof(Details), "Pedidos", new { id });
                    }

                }

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
                .Include(p => p.DetallePedidos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido != null)
            {
                pedido.EstadoId = 5;
                pedido.FechaAnulado = DateTime.Now;
                _context.Update(pedido);

                foreach (var dp in pedido.DetallePedidos)
                {
                    // Devolver el stock a los productos
                    var producto = await _context.Productos.FindAsync(dp.ProductoId);

                    if (producto == null) continue;

                    producto.Stock += dp.Cantidad;
                    _context.Update(producto);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), "Pedidos", new { id });
        }

        // POST /Pedidos/Enviado
        [HttpPost, ActionName("Enviar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnviarPedido(int id)
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
        public async Task<IActionResult> EntregarPedido(int id)
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
        public async Task<IActionResult> DevolverPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedidos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pedido != null)
            {
                pedido.EstadoId = 6;
                pedido.FechaDevolucion = DateTime.Now;
                _context.Update(pedido);

                foreach (var dp in pedido.DetallePedidos)
                {
                    // Devolver el stock a los productos
                    var producto = await _context.Productos.FindAsync(dp.ProductoId);

                    if (producto == null) continue;

                    producto.Stock += dp.Cantidad;
                    _context.Update(producto);
                }

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
                .Include(dp => dp.Producto)
                .Where(dp => dp.PedidoId == id)
                .FirstOrDefaultAsync(dp => dp.Id == dpid);

            Pedido? pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

            if (dp.Cantidad < dp.Producto.Stock)
            {
                dp.Cantidad++;
                _context.Update(dp);
                await CalcDescuentos(id, pedido.CodDescuento);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Carrito", "Pedidos", new { id });
        }

        // POST /Pedidos/Menos
        [HttpPost, ActionName("Menos")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MenosPedido(int id, int dpid)
        {
            DetallePedido? dp = await _context.DetallePedidos
                .Where(dp => dp.PedidoId == id)
                .FirstOrDefaultAsync(dp => dp.Id == dpid);
            Pedido? pedido = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

            if (dp.Cantidad > 1)
            {
                dp.Cantidad--;
                _context.Update(dp);
                await CalcDescuentos(id, pedido.CodDescuento);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Carrito", "Pedidos", new { id });
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
