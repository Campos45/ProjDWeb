using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentarioApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComentarioApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            return await _context.Comentario.Include(c => c.Imagem).Include(c => c.Utilizador).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario == null) return NotFound();

            return comentario;
        }

        [HttpPost]
        public async Task<ActionResult<Comentario>> PostComentario(Comentario comentario)
        {
            comentario.Data = DateTime.Now;
            _context.Comentario.Add(comentario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComentario), new { id = comentario.Id }, comentario);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario == null) return NotFound();

            _context.Comentario.Remove(comentario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}