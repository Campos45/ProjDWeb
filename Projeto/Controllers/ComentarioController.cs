using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Criar([Bind("ComentarioTexto,ImagemId")] Comentario comentario)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized("Utilizador não autenticado.");
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == user.UserName);

            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado na base de dados.");
            }

            comentario.Data = DateTime.Now;
            comentario.Utilizador = utilizador;

            _context.Add(comentario);
            await _context.SaveChangesAsync();

            // Redirecionar de volta para os detalhes do monumento com a imagem
            var imagem = await _context.Imagem
                .Include(i => i.Monumento)
                .FirstOrDefaultAsync(i => i.Id == comentario.ImagemId);

            if (imagem == null)
            {
                return NotFound("Imagem não encontrada.");
            }

            return RedirectToAction("Details", "Monumento", new { id = imagem.MonumentoId });
        }
    }
}