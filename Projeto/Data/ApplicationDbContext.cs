using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using appMonumentos.Models;

namespace WebApplication1.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<appMonumentos.Models.Monumento> Monumento { get; set; } = default!;

public DbSet<appMonumentos.Models.Utilizador> Utilizador { get; set; } = default!;

public DbSet<appMonumentos.Models.Localidade> Localidade { get; set; } = default!;

public DbSet<appMonumentos.Models.Imagem> Imagem { get; set; }

}