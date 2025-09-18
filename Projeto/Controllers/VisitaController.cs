using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using appMonumentos.Models;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    /// Controlador que gere as visitas de utilizadores a monumentos
    [Authorize] // apenas utilizadores autenticados podem usar
    public class VisitaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VisitaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        ///POST: Alterna visita (se existe remove, se não existe adiciona)
        [HttpPost]
        public async Task<IActionResult> ToggleVisita(int monumentoId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == user.UserName);
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == monumentoId && v.UtilizadorId == utilizador.Id);

            if (visita != null)
            {
                // Se já existir, verificar permissões
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin && visita.UtilizadorId != utilizador.Id)
                    return Forbid();

                // Remover (toggle off)
                _context.VisitaMonumento.Remove(visita);
            }
            else
            {
                // Criar nova visita (toggle on)
                _context.VisitaMonumento.Add(new VisitaMonumento
                {
                    MonumentoId = monumentoId,
                    UtilizadorId = utilizador.Id
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        /// GET: Lista de visitantes de um monumento
        [HttpGet]
        public async Task<IActionResult> ListaVisitantes(int monumentoId)
        {
            var visitantes = await _context.VisitaMonumento
                .Include(v => v.Utilizador)
                .Where(v => v.MonumentoId == monumentoId)
                .Select(v => v.Utilizador.Nome)
                .ToListAsync();

            // Retorna uma partial view com nomes
            return PartialView("_ListaVisitantesPartial", visitantes);
        }
    }
}
