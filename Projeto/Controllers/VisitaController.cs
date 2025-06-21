// Controllers/VisitaController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.AspNetCore.Identity;
using appMonumentos.Models;

namespace WebApplication1.Controllers
{
    public class VisitaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public VisitaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleVisita(int monumentoId)
        {
            var user = await _userManager.GetUserAsync(User);
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == user.UserName);

            if (utilizador == null)
                return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == monumentoId && v.UtilizadorId == utilizador.Id);

            if (visita != null)
            {
                _context.VisitaMonumento.Remove(visita);
            }
            else
            {
                _context.VisitaMonumento.Add(new VisitaMonumento
                {
                    MonumentoId = monumentoId,
                    UtilizadorId = utilizador.Id
                });
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ListaVisitantes(int monumentoId)
        {
            var visitantes = await _context.VisitaMonumento
                .Include(v => v.Utilizador)
                .Where(v => v.MonumentoId == monumentoId)
                .Select(v => v.Utilizador.Nome)
                .ToListAsync();

            return PartialView("_ListaVisitantesPartial", visitantes);
        }
    }
}
