using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

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
// 🔑 Configuração JWT
// =======================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];

// =======================
// 🔑 Configuração de Autenticação
// =======================
builder.Services.AddAuthentication()
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false; // em dev
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });



// =======================
// Swagger com suporte JWT
// =======================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Monumentos",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// =======================
// Controladores e JSON
// =======================
builder.Services.AddControllersWithViews();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// =======================
// Configuração do Pipeline
// =======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
// Criação do admin, user normal e roles
// =======================
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Garantir roles
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("Utilizador"))
        await roleManager.CreateAsync(new IdentityRole("Utilizador"));

    // =======================
    // Criar utilizador Admin
    // =======================
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

            // Inserir também na tabela personalizada Utilizador
            if (!context.Utilizador.Any(u => u.Username == adminUser.UserName))
            {
                var utilizador = new Utilizador
                {
                    Username = adminUser.UserName,
                    Nome = "Administrador",
                    Email = adminEmail,
                    LocalidadeUtilizador = "N/D",
                    Password = "IdentityManaged" // <- dummy
                };
                context.Utilizador.Add(utilizador);
                await context.SaveChangesAsync();
            }
        }
    }

    // =======================
    // Criar utilizador normal
    // =======================
    var userEmail = "user@gmail.com";
    var normalUser = await userManager.FindByEmailAsync(userEmail);
    if (normalUser == null)
    {
        normalUser = new IdentityUser
        {
            UserName = "UserNormal",
            Email = userEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(normalUser, "aA-123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(normalUser, "Utilizador");

            // Inserir também na tabela personalizada Utilizador
            if (!context.Utilizador.Any(u => u.Username == normalUser.UserName))
            {
                var utilizador = new Utilizador
                {
                    Username = normalUser.UserName,
                    Nome = "Utilizador",
                    Email = userEmail,
                    LocalidadeUtilizador = "N/D",
                    Password = "IdentityManaged" // <- dummy
                };
                context.Utilizador.Add(utilizador);
                await context.SaveChangesAsync();
            }
        }
    }
}


// =======================
app.Run();
