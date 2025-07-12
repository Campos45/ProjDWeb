using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Controllers.API
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class MonumentoApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MonumentoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/monumentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Monumento>>> GetMonumentos()
        {
            return await _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador)
                .ToListAsync();
        }

        // GET: api/monumentos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Monumento>> GetMonumento(int id)
        {
            var monumento = await _context.Monumento
                .Include(m => m.Utilizador)
                .Include(m => m.Localidade)
                .Include(m => m.Imagens)
                .ThenInclude(i => i.Comentarios)
                .Include(m => m.VisitasMonumento)
                .ThenInclude(v => v.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null)
            {
                return NotFound();
            }

            return monumento;
        }

        // POST: api/monumentos
        [HttpPost]
        public async Task<ActionResult<Monumento>> PostMonumento(Monumento monumento)
        {
            _context.Monumento.Add(monumento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMonumento), new { id = monumento.Id }, monumento);
        }

        // PUT: api/monumentos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMonumento(int id, Monumento monumento)
        {
            if (id != monumento.Id)
            {
                return BadRequest();
            }

            _context.Entry(monumento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Monumento.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: api/monumentos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonumento(int id)
        {
            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null)
            {
                return NotFound();
            }

            _context.Monumento.Remove(monumento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
