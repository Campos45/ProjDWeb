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
    // Controller para gerir as visitas dos utilizadores a monumentos
    // Apenas utilizadores autenticados podem aceder (Authorize)
    [Authorize]
    public class VisitaController : Controller
    {
        private readonly ApplicationDbContext _context;  // Contexto da base de dados
        private readonly UserManager<IdentityUser> _userManager; // Gestão de utilizadores do Identity

        // Construtor para injetar dependências necessárias
        public VisitaController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Método que alterna a visita de um utilizador a um monumento (toggle)
        [HttpPost]
        public async Task<IActionResult> ToggleVisita(int monumentoId)
        {
            // Obter o utilizador atualmente autenticado
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized(); // Não autenticado

            // Obter os dados adicionais do utilizador na tabela Utilizador
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == user.UserName);
            if (utilizador == null)
                return Unauthorized(); // Utilizador não encontrado na BD personalizada

            // Verificar se já existe uma visita registada para este utilizador e monumento
            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == monumentoId && v.UtilizadorId == utilizador.Id);

            if (visita != null)
            {
                // Se já existir visita, verifica se pode remover (admin ou próprio utilizador)
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin && visita.UtilizadorId != utilizador.Id)
                    return Forbid(); // Proibido para utilizadores que não são admin e não são donos da visita

                // Remove a visita (toggle OFF)
                _context.VisitaMonumento.Remove(visita);
            }
            else
            {
                // Caso não exista visita, adiciona uma nova visita (toggle ON)
                _context.VisitaMonumento.Add(new VisitaMonumento
                {
                    MonumentoId = monumentoId,
                    UtilizadorId = utilizador.Id
                });
            }

            // Guarda as alterações na base de dados
            await _context.SaveChangesAsync();

            // Resposta OK para o cliente (por exemplo, chamada AJAX)
            return Ok();
        }

        // GET: Obter a lista de nomes dos visitantes de um monumento
        [HttpGet]
        public async Task<IActionResult> ListaVisitantes(int monumentoId)
        {
            // Buscar na tabela VisitaMonumento os visitantes deste monumento
            // Incluir os dados do utilizador para obter o nome
            var visitantes = await _context.VisitaMonumento
                .Include(v => v.Utilizador)  // inclui dados do utilizador relacionado
                .Where(v => v.MonumentoId == monumentoId)  // filtra pelo monumento
                .Select(v => v.Utilizador.Nome)  // seleciona apenas o nome do utilizador
                .ToListAsync();

            // Retorna uma partial view que mostra a lista de visitantes
            return PartialView("_ListaVisitantesPartial", visitantes);
        }
    }
}
