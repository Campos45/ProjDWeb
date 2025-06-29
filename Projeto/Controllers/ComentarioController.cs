using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers
{
    public class ComentarioController : Controller
    {
        // Injeção de dependências: contexto da base de dados e gestor de utilizadores Identity
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Construtor do controlador
        public ComentarioController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Criar um novo comentário
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Apenas utilizadores autenticados podem comentar
        public async Task<IActionResult> Criar([Bind("ComentarioTexto,ImagemId")] Comentario comentario)
        {
            // Obtem o utilizador autenticado
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("Utilizador não autenticado.");

            // Procura o utilizador associado na tabela personalizada 'Utilizador'
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == user.UserName);
            if (utilizador == null) return NotFound("Utilizador não encontrado na base de dados.");

            // Define a data atual no comentário e associa o utilizador autenticado
            comentario.Data = DateTime.Now;
            comentario.Utilizador = utilizador;

            // Adiciona o comentário à base de dados e guarda
            _context.Add(comentario);
            await _context.SaveChangesAsync();

            // Obtém a imagem associada ao comentário (com o monumento incluído)
            var imagem = await _context.Imagem
                .Include(i => i.Monumento)
                .FirstOrDefaultAsync(i => i.Id == comentario.ImagemId);
            if (imagem == null) return NotFound("Imagem não encontrada.");

            // Redireciona para a página de detalhes do monumento
            return RedirectToAction("Details", "Monumento", new { id = imagem.MonumentoId });
        }

        // POST: Apagar um comentário
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize] // Apenas utilizadores autenticados podem apagar comentários
        public async Task<IActionResult> Delete(int id)
        {
            // Procura o comentário, incluindo a imagem e o monumento a que pertence
            var comentario = await _context.Comentario
                .Include(c => c.Imagem)
                    .ThenInclude(i => i.Monumento)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comentario == null) return NotFound();

            // Obtém o utilizador autenticado
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Procura o utilizador na tabela 'Utilizador'
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == user.UserName);

            // Verifica se o utilizador é administrador
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // Se não for admin e não for o autor do comentário, nega o acesso
            if (!isAdmin && comentario.UtilizadorId != utilizador?.Id)
                return Forbid();

            // Remove o comentário da base de dados e guarda alterações
            _context.Comentario.Remove(comentario);
            await _context.SaveChangesAsync();

            // Redireciona para a página de detalhes do monumento correspondente
            return RedirectToAction("Details", "Monumento", new { id = comentario.Imagem.MonumentoId });
        }
    }
}
