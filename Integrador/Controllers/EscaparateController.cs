using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Integrador.Controllers
{
    public class EscaparateController(IntegradorContexto context) : Controller
    {

        private readonly IntegradorContexto _context = context;

        // GET /Escaparate/Index
        public async Task<IActionResult> Index(int? MarcaId)
        {
            if (User.IsInRole("Cliente"))
            {
                Cliente? cliente = await _context.Clientes
                    .Where(c => c.Email == User.Identity.Name)
                    .FirstOrDefaultAsync();

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
            }

            ViewData["ListaMarcas"] = new SelectList(_context.Marcas, "Id", "Nombre");

            var productos = _context.Productos
                .Include(p => p.Modelo)
                .Where(p => p.Escaparate);
            /*.Where(p => p.Escaparate && p.MarcaId == MarcaId);*/

            return View(await productos.ToListAsync());
        }

        // GET /Escaparate/AgregarCarrito
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AgregarCarrito(int? id)
        {
            if (id == 0) return RedirectToAction(nameof(Index), "Escaparate");

            var producto = await _context.Productos
                .Include(p => p.Modelo)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            return View(producto);
        }

        // POST /Escaparate/AgregarCarrito
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarCarrito(int id, int cantidad = 1)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (cantidad <= 0)
            {
                ModelState.AddModelError(string.Empty, "La cantidad tiene que ser un número entero positivo.");

                producto = await _context.Productos
                .Include(p => p.Modelo)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

                return View(producto);
            }
            else if (cantidad > producto.Stock)
            {
                ModelState.AddModelError(string.Empty, "La cantidad tiene que ser menor al stock disponible.");

                producto = await _context.Productos
                .Include(p => p.Modelo)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

                return View(producto);
            }

            if (producto == null) return NotFound();

            if (HttpContext.Session.GetString("NumPedido") == null)
            {
                Cliente? cliente = await _context.Clientes
                .Where(c => c.Email == User.Identity.Name)
                .FirstOrDefaultAsync();

                Pedido pedido = new()
                {
                    ClienteId = cliente.Id,
                    EstadoId = 1,
                    FechaPedido = DateTime.Now,
                    Descuento = 0
                };

                if (ModelState.IsValid)
                {
                    _context.Add(pedido);
                    await _context.SaveChangesAsync();
                }

                HttpContext.Session.SetString("NumPedido", pedido.Id.ToString());
            }

            DetallePedido? dpd = await _context.DetallePedidos
                .Where(dp => dp.PedidoId == Convert.ToInt32(HttpContext.Session.GetString("NumPedido")))
                .Where(dp => dp.ProductoId == producto.Id)
                .FirstOrDefaultAsync();

            if (dpd != null)
            {
                dpd.Cantidad += cantidad;
                if (ModelState.IsValid)
                {
                    _context.Update(dpd);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                dpd = new()
                {
                    // Convert.ToInt32(producto.PedidoId),
                    PedidoId = Convert.ToInt32(HttpContext.Session.GetString("NumPedido")),
                    Cantidad = cantidad,
                    ProductoId = producto.Id,
                    PrecioUnidad = producto.Precio,
                };
                if (ModelState.IsValid)
                {
                    _context.Add(dpd);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction("Carrito", "Pedidos");
        }
    }
}
