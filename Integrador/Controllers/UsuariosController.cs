using Integrador.Areas.Identity.Pages.Account;
using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Integrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController(IntegradorContexto contexto, ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IntegradorContexto _contexto = contexto;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;

        public IActionResult Index()
        {
            var usuarios = _context.Users
                    .Join(_context.UserRoles, user => user.Id, userRoles => userRoles.UserId, (user, userRoles) => new { user, userRoles })
                    .Join(_context.Roles, temp => temp.userRoles.RoleId, role => role.Id, (temp, role) => new { temp.user, temp.userRoles, role })
                    .Select(temp => new ViewUsuarios
                    {
                        Email = temp.user.Email,
                        RolDeUsuario = temp.role.Name
                    });

            return View(usuarios.ToList());
        }

        //GET: Usuarios/Create
        public IActionResult Create()
        {
            return View();
        }

        //POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Password,ConfirmPassword")]
                                                RegisterModel.InputModel model)
        {
            var user = new IdentityUser();
            user.Email = user.UserName = model.Email;

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Administrador");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordRequiresDigit")
                {
                    ModelState.AddModelError("Password", "La contraseña debe tener un número.");
                }
                else if (error.Code == "DuplicateUserName")
                {
                    ModelState.AddModelError("Email", "El email ya está registrado.");
                }
                else
                {
                    Console.WriteLine(error.Code); // Quitar comentario
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }

            return View(model);
        }

        // POST: Usuarios/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            var user = await _userManager.FindByEmailAsync(userId);

            if (user == null)
            {
                return NotFound($"Unable to find user with ID '{userId}'.");
            }

            // Comprobar el rol del usuario que se va a borrar
            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("Proveedor"))
            {
                Proveedor? proveedor = await _contexto.Proveedores
                .Where(p => p.Email == user.Email)
                .FirstOrDefaultAsync();

                if (proveedor != null)
                {
                    return RedirectToAction("Delete", "Proveedores", new { id = proveedor.Id, admin = true });
                }
            }
            else if (userRoles.Contains("Cliente"))
            {
                Cliente? cliente = await _contexto.Clientes
                    .Where(c => c.Email == user.Email)
                    .FirstOrDefaultAsync();

                if (cliente != null)
                {
                    return RedirectToAction("Delete", "Clientes", new { id = cliente.Id, admin = true });
                }
            }

            var currentUserId = User.Identity.Name;

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                // Manejar el error en caso de que no se pueda eliminar al usuario
                return BadRequest(result.Errors);
            }

            if (userId == currentUserId)
            {
                // Cerrar la sesión del usuario actual
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
        }

        private bool IdentityUserExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }
    }
}
