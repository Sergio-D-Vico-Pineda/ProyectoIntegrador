using Integrador;
using Integrador.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Cambio de carpeta de la base de datos
string path = Path.Combine(Directory.GetCurrentDirectory(), "DataBase");
AppDomain.CurrentDomain.SetData("DataDirectory", path);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar el contexto a la base de datos
builder.Services.AddDbContext<IntegradorContexto>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// Configuraci�n de los servicios de ASP.NET Core Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings. Configuraci�n de las caracter�sticas de las contrase�as
    options.Password.RequireDigit = true; // Un numero
    options.Password.RequireLowercase = true; // Min�scula
    options.Password.RequireNonAlphanumeric = false; // No s�
    options.Password.RequireUppercase = false; // May�scula
    options.Password.RequiredLength = 6; // Longitud m�nima
    options.Password.RequiredUniqueChars = 1; // Caracteres diferentes
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Escaparate}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.InitializeAsync(services).Wait();
}

app.Run();
