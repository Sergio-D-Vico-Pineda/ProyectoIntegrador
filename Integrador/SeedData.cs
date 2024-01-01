using Microsoft.AspNetCore.Identity;

namespace Integrador
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            // Comprobar y crear los roles predeterminados
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await CrearRolesAsync(roleManager);

            // Comprobar y crear el administrador predeterminado
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            await CrearAdminAsync(userManager);
        }

        private static async Task CrearRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // Si no existe, se crea el rol predeterminado "Administrador"
            string nombreRol = "Administrador";
            var Existe = await roleManager.RoleExistsAsync(nombreRol);
            if (!Existe)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));

            // Si no existe, se crea el rol predeterminado "Cliente"
            nombreRol = "Cliente";
            Existe = await roleManager.RoleExistsAsync(nombreRol);
            if (!Existe)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));

            // Si no existe, se crea el rol predeterminado "Proveedor"
            nombreRol = "Proveedor";
            Existe = await roleManager.RoleExistsAsync(nombreRol);
            if (!Existe)
                await roleManager.CreateAsync(new IdentityRole(nombreRol));
        }

        private static async Task CrearAdminAsync(UserManager<IdentityUser> userManager)
        {
            // Comprobar si existe el administrador predetermindado
            var testAdmin = userManager.Users
            .Where(x => x.UserName == "a@a.com")
            .SingleOrDefault();

            // Si existe sale de la función
            if (testAdmin != null) return;

            // Si no existe, se crea el administrador predeterminado "admin@empresa.com"
            testAdmin = new IdentityUser
            {
                UserName = "a@a.com",
                Email = "a@a.com"
            };

            string admPasswd = "admin1";

            IdentityResult userResult = await userManager.CreateAsync(testAdmin, admPasswd);

            // Se agrega el rol "Administrador" al administrador predeterminado 
            if (userResult.Succeeded)
            {
                await userManager.AddToRoleAsync(testAdmin, "Administrador");
            }
        }
    }
}
