using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class UtilizadorApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UtilizadorApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilizador>>> GetUtilizadores()
        {
            return await _context.Utilizador.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Utilizador>> GetUtilizador(int id)
        {
            var utilizador = await _context.Utilizador.FindAsync(id);

            if (utilizador == null) return NotFound();
            return utilizador;
        }

        [HttpPost]
        public async Task<ActionResult<Utilizador>> PostUtilizador(Utilizador utilizador)
        {
            _context.Utilizador.Add(utilizador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUtilizador), new { id = utilizador.Id }, utilizador);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilizador(int id, Utilizador utilizador)
        {
            if (id != utilizador.Id) return BadRequest();

            _context.Entry(utilizador).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilizador(int id)
        {
            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null) return NotFound();

            _context.Utilizador.Remove(utilizador);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}