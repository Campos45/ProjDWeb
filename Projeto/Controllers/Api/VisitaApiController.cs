using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class VisitaApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VisitaApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VisitaMonumento>>> GetVisitas()
        {
            return await _context.VisitaMonumento.Include(v => v.Utilizador).Include(v => v.Monumento).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VisitaMonumento>> GetVisita(int id)
        {
            var visita = await _context.VisitaMonumento.FindAsync(id);
            if (visita == null) return NotFound();

            return visita;
        }

        [HttpPost]
        public async Task<ActionResult<VisitaMonumento>> PostVisita(VisitaMonumento visita)
        {
            _context.VisitaMonumento.Add(visita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVisita), new { id = visita.Id }, visita);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisita(int id)
        {
            var visita = await _context.VisitaMonumento.FindAsync(id);
            if (visita == null) return NotFound();

            _context.VisitaMonumento.Remove(visita);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}