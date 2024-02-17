using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace Integrador.Controllers
{
    public class EscaparateController(IntegradorContexto context) : Controller
    {

        private readonly IntegradorContexto _context = context;

        // GET /Escaparate/Index
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index(int? id, int? mid, string? busq)
        {
            ViewBag.busq = busq;

            ViewData["ListaMarcas"] = _context.Marcas
                .Include(m => m.Modelos)
                .ThenInclude(m => m.Productos)
                .ToList();

            var productos = _context.Productos.AsQueryable();

            if (!String.IsNullOrEmpty(busq))
            {
                productos = productos
                    .Where(p => p.Nombre.Contains(busq) || p.Descripcion.Contains(busq) || p.Modelo.Nombre.Contains(busq));
            }

            if (id != null)
            {
                Marca? marca = await _context.Marcas.FirstOrDefaultAsync(m => m.Id == id);

                if (marca == null) return RedirectToAction(nameof(Index), new { id = "" });

                ViewBag.marca = marca.Nombre;
                productos = productos.Where(p => p.Modelo.MarcaId == id);

                Modelo? modelo = await _context.Modelos.FirstOrDefaultAsync(m => m.Id == mid);

                if (modelo != null)
                {
                    ViewBag.modelo = modelo.Nombre;
                    productos = productos.Where(p => p.ModeloId == mid);
                }
            }

            if (User.IsInRole("Cliente"))
            {
                // Obtener el cliente actual
                Cliente? cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);

                // Si el cliente no existe, redirigir a la vista de creación de cuenta
                if (cliente == null) return RedirectToAction(nameof(Create), "MisDatos", new { role = "Cliente" });

                var numPed = HttpContext.Session.GetString("NumPedido");
                if (numPed != null)
                {
                    Pedido? lastpedido = await _context.Pedidos
                        .Where(p => p.EstadoId == 1)
                        .Where(p => p.ClienteId == cliente.Id)
                        .Where(p => p.Id == Convert.ToInt32(numPed))
                        .Include(p => p.DetallePedidos)
                        .FirstOrDefaultAsync();

                    ViewBag.carrito = lastpedido.DetallePedidos.Select(d => d.ProductoId).ToArray();
                }
            }

            productos = productos
                .Include(p => p.Modelo)
                .Where(p => p.Escaparate)
                .OrderByDescending(p => p.Id);

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
        public async Task<IActionResult> AgregarCarrito(int id, int? cantidad = 1)
        {
            var producto = await _context.Productos.FindAsync(id);

            if (producto == null) return NotFound();

            if (cantidad <= 0 || cantidad == null)
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

            Cliente? cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);

            if (HttpContext.Session.GetString("NumPedido") == null)
            {
                Pedido pedido = new()
                {
                    ClienteId = cliente.Id,
                    EstadoId = 1,
                    FechaPedido = DateTime.Now
                };

                if (ModelState.IsValid)
                {
                    _context.Add(pedido);
                    await _context.SaveChangesAsync();
                }

                HttpContext.Session.SetString("NumPedido", pedido.Id.ToString());
            }
            else
            {
                // El pedido ya existe
                Pedido? pedido1 = await _context.Pedidos
                    .Where(p => p.Id == Convert.ToInt32(HttpContext.Session.GetString("NumPedido")))
                    .FirstOrDefaultAsync();

                if (pedido1.ClienteId != cliente.Id)
                {
                    Pedido pedido = new()
                    {
                        ClienteId = cliente.Id,
                        EstadoId = 1,
                        FechaPedido = DateTime.Now
                    };

                    if (ModelState.IsValid)
                    {
                        _context.Add(pedido);
                        await _context.SaveChangesAsync();
                    }

                    HttpContext.Session.SetString("NumPedido", pedido.Id.ToString());
                }
            }

            DetallePedido? dpd = await _context.DetallePedidos
                .Where(dp => dp.PedidoId == Convert.ToInt32(HttpContext.Session.GetString("NumPedido")))
                .Where(dp => dp.ProductoId == producto.Id)
                .FirstOrDefaultAsync();

            if (dpd != null)
            {
                dpd.Cantidad += (int)cantidad;
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
                    PedidoId = Convert.ToInt32(HttpContext.Session.GetString("NumPedido")),
                    Cantidad = (int)cantidad,
                    ProductoId = producto.Id,
                    PrecioUnidad = producto.Precio,
                };
                if (ModelState.IsValid)
                {
                    _context.Add(dpd);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
