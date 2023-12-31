using Integrador.Data;
using Integrador.Models;
using Integrador.Views.MisDatos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Integrador.Controllers
{
    public class MisDatosController : Controller
    {
        private readonly IntegradorContexto _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<MisDatosController> _logger;


        public MisDatosController(IntegradorContexto context,
                                  UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager,
                                  ILogger<MisDatosController> logger)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // PROVEEDORES

        // GET: MisDatos/CreatePro
        [Authorize(Roles = "Proveedor")]
        public IActionResult CreatePro()
        {
            return View();
        }

        // POST: MisDatos/CreatePro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreatePro(
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Proveedor proveedor)
        {
            proveedor.Email = User.Identity.Name;

            if (ModelState.IsValid)
            {
                _context.Add(proveedor);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: MisDatos/EditPro
        [Authorize(Roles = "Proveedor")]
        public async Task<IActionResult> EditPro()
        {
            string? email = User.Identity.Name;

            Proveedor? proveedor = await _context.Proveedores
                .Where(p => p.Email == email)
                .FirstOrDefaultAsync();

            if (proveedor == null) return NotFound();

            return View(proveedor);
        }

        // POST: MisDatos/EditPro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPro(int id,
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Proveedor proveedor)
        {
            if (id != proveedor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.Id))
                    {
                        return NotFound();
                    }
                    else
                        throw;

                }

                return RedirectToAction("Index", "Home");
            }

            return View(proveedor);
        }

        private bool ProveedorExists(int id)
        {
            return (_context.Proveedores?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // CLIENTES

        // GET: MisDatos/CreateCli
        [Authorize(Roles = "Cliente")]
        public IActionResult CreateCli()
        {
            return View();
        }

        // POST: MisDatos/CreateCli
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCli(
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Cliente cliente)
        {
            cliente.Email = User.Identity.Name;

            if (ModelState.IsValid)
            {
                _context.Add(cliente);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Escaparate");
            }

            return View();
        }

        // GET: MisDatos/EditCli
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> EditCli()
        {
            string? email = User.Identity.Name;

            Cliente? cliente = await _context.Clientes
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync();

            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: MisDatos/EditCli
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCli(int id,
            [Bind("Id,Nombre,Email,Nif,Telefono,Direccion")] Cliente cliente)
        {
            if (id != cliente.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClienteExists(cliente.Id))
                    {
                        return NotFound();
                    }
                    else
                        throw;

                }
                return RedirectToAction("Index", "Escaparate");
            }

            return View(cliente);
        }

        private bool ClienteExists(int id)
        {
            return (_context.Clientes?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // GET: MisDatos/ChangePassword

        public async Task<IActionResult> ChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction("SetPassword");
            }
            return View();
        }

        // POST: MisDatos/ChangePassword

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel.InputModel input)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {

                ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");

                return View();
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");

            ViewData["Status"] = "Tu contraseña ha sido cambiada.";

            return View();
        }

        public async Task<IActionResult> DeletePersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ViewBag.Require = await _userManager.HasPasswordAsync(user);
            return View();
        }

    }
}
