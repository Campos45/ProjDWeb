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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ComentarioController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: Criar Comentário
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Criar([Bind("ComentarioTexto,ImagemId")] Comentario comentario)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized("Utilizador não autenticado.");

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == user.UserName);
            if (utilizador == null) return NotFound("Utilizador não encontrado na base de dados.");

            comentario.Data = DateTime.Now;
            comentario.Utilizador = utilizador;

            _context.Add(comentario);
            await _context.SaveChangesAsync();

            var imagem = await _context.Imagem
                .Include(i => i.Monumento)
                .FirstOrDefaultAsync(i => i.Id == comentario.ImagemId);
            if (imagem == null) return NotFound("Imagem não encontrada.");

            return RedirectToAction("Details", "Monumento", new { id = imagem.MonumentoId });
        }

        // POST: Delete Comentário
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comentario = await _context.Comentario
                .Include(c => c.Imagem)
                    .ThenInclude(i => i.Monumento)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (comentario == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == user.UserName);
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (!isAdmin && comentario.UtilizadorId != utilizador?.Id)
                return Forbid();

            _context.Comentario.Remove(comentario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Monumento", new { id = comentario.Imagem.MonumentoId });
        }
    }
}
