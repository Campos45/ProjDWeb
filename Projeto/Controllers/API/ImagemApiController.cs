using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagemApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImagemApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Imagem>>> GetImagens()
        {
            return await _context.Imagem.Include(i => i.Utilizador).Include(i => i.Monumento).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Imagem>> GetImagem(int id)
        {
            var imagem = await _context.Imagem.Include(i => i.Comentarios).FirstOrDefaultAsync(i => i.Id == id);

            if (imagem == null) return NotFound();
            return imagem;
        }

        [HttpPost]
        public async Task<ActionResult<Imagem>> PostImagem(Imagem imagem)
        {
            _context.Imagem.Add(imagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetImagem), new { id = imagem.Id }, imagem);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImagem(int id)
        {
            var imagem = await _context.Imagem.FindAsync(id);
            if (imagem == null) return NotFound();

            _context.Imagem.Remove(imagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}