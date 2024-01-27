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
    [Authorize(Roles = "Proveedor, Cliente")]
    public class MisDatosController(IntegradorContexto context,
                              UserManager<IdentityUser> userManager,
                              SignInManager<IdentityUser> signInManager,
                              ILogger<MisDatosController> logger) : Controller
    {
        private readonly IntegradorContexto _context = context;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;
        private readonly ILogger<MisDatosController> _logger = logger;


        // GET: MisDatos/Create
        public IActionResult Create(string role)
        {
            if (role == "Proveedor")
                return View("CreatePro");
            else if (role == "Cliente")
                return View("CreateCli");
            else
                return NotFound();
        }

        public async Task<IActionResult> Index()
        {
            // HOME CONTROLLER 
            string? email = User.Identity.Name;

            Proveedor? proveedor = await _context.Proveedores
                .Where(p => p.Email == email)
                .FirstOrDefaultAsync();

            Cliente? cliente = await _context.Clientes
                .Where(c => c.Email == email)
                .FirstOrDefaultAsync();

            if (proveedor == null && cliente == null)
            {
                if (User.IsInRole("Proveedor") && !_context.Proveedores.Any(p => p.Email == email))
                {
                    return RedirectToAction("Create", "MisDatos", new { role = "Proveedor" });
                }

                if (User.IsInRole("Cliente") && !_context.Clientes.Any(c => c.Email == email))
                {
                    return RedirectToAction("Create", "MisDatos", new { role = "Cliente" });
                }
                return NotFound();
            }

            if (proveedor != null)
                return View("EditPro", proveedor);
            else
                return View("EditCli", cliente);

        }

        // PROVEEDORES

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePersonalData(DeletePersonalDataModel.InputModel input)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (await _userManager.HasPasswordAsync(user) && !await _userManager.CheckPasswordAsync(user, input.Password))
            {
                ModelState.AddModelError(string.Empty, "La contraseña es incorrecta.");
                ViewBag.Require = true;
                return View();
            }

            if (User.IsInRole("Proveedor"))
            {
                var pro = await _context.Proveedores.Where(p => p.Email == user.Email).FirstOrDefaultAsync();

                if (pro != null)
                {
                    pro.Email += "-DEL.";
                    _context.Update(pro);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound($"Unable to load user with ID '{user.Email}'.");
                }
            }
            else if (User.IsInRole("Cliente"))
            {
                var cli = await _context.Clientes.Where(c => c.Email == user.Email).FirstOrDefaultAsync();
                if (cli != null)
                {
                    cli.Email += "-DEL.";
                    _context.Update(cli);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound($"Unable to load user with ID '{user.Email}'.");
                }
            }

            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error inesperado borrando usuario.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }
    }
}
