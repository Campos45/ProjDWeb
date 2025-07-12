using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Base de Dados + Identity
// =======================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// =======================
// Controladores e Swagger
// =======================
builder.Services.AddControllersWithViews(); // MVC Views
//builder.Services.AddControllers();          // API Controllers

builder.Services.AddEndpointsApiExplorer(); // Necessário para Swagger
builder.Services.AddSwaggerGen();           // Gera documentação da API

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });


var app = builder.Build();

// =======================
// Configuração do Pipeline
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();    // JSON da API
    app.UseSwaggerUI();  // Interface gráfica do Swagger
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// =======================
// Mapas de Rotas
// =======================
app.MapControllers(); // APIs
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// =======================
// Criação do admin e da role
// =======================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Criar role Admin se ainda não existir
    var roleExists = await roleManager.RoleExistsAsync("Admin");
    if (!roleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Criar utilizador admin se ainda não existir
    var adminEmail = "admin@admin.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = "UtilizadorAdmin",
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");

            var utilizador = new Utilizador
            {
                Username = adminUser.UserName,
                Nome = "Administrador",
                Email = adminEmail,
                Password = "Admin123!"
            };

            context.Utilizador.Add(utilizador);
            await context.SaveChangesAsync();
        }
    }
}

// =======================
app.Run();
