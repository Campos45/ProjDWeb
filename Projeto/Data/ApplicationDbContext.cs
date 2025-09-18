using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using appMonumentos.Models;

namespace WebApplication1.Data
{
    // Contexto principal da aplicação (inclui Identity e tabelas personalizadas)
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets correspondem a tabelas na BD
        public DbSet<Monumento> Monumento { get; set; } = default!;
        public DbSet<Utilizador> Utilizador { get; set; } = default!;
        public DbSet<Localidade> Localidade { get; set; } = default!;
        public DbSet<Imagem> Imagem { get; set; } = default!;
        public DbSet<Comentario> Comentario { get; set; } = default!;
        public DbSet<Caracteristicas> Caracteristicas { get; set; } = default!;
        public DbSet<VisitaMonumento> VisitaMonumento { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Criar índice único para evitar visitas duplicadas (mesmo utilizador no mesmo monumento)
            modelBuilder.Entity<VisitaMonumento>()
                .HasIndex(v => new { v.MonumentoId, v.UtilizadorId })
                .IsUnique();
        }
    }
}