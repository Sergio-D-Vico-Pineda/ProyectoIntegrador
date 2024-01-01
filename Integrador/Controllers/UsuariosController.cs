using Integrador.Areas.Identity.Pages.Account;
using Integrador.Data;
using Integrador.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Integrador.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : Controller
    {
        private readonly ApplicationDbContext _context = context;
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
        public async Task<IActionResult> Create([Bind("Email,Password")]
                                                RegisterModel.InputModel model)
        {
            var user = new IdentityUser();
            user.Email = user.UserName = model.Email;
            string password = model.Password;

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Administrador");
                return RedirectToAction(nameof(Index));
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
    }
}
