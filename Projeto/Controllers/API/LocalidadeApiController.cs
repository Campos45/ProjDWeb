using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalidadeApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocalidadeApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Localidade
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Localidade>>> GetLocalidades()
        {
            return await _context.Localidade.ToListAsync();
        }

        // GET: api/Localidade/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Localidade>> GetLocalidade(int id)
        {
            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade == null)
            {
                return NotFound();
            }
            return localidade;
        }

        // POST: api/Localidade
        [HttpPost]
        public async Task<ActionResult<Localidade>> PostLocalidade(Localidade localidade)
        {
            _context.Localidade.Add(localidade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocalidade), new { id = localidade.Id }, localidade);
        }

        // PUT: api/Localidade/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocalidade(int id, Localidade localidade)
        {
            if (id != localidade.Id)
                return BadRequest();

            _context.Entry(localidade).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Localidade.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Localidade/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocalidade(int id)
        {
            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade == null) return NotFound();

            _context.Localidade.Remove(localidade);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
