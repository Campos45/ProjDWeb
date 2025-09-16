using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class VisitaApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VisitaApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/VisitaApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVisitas()
        {
            var visitas = await _context.VisitaMonumento
                .Select(v => new
                {
                    v.Id,
                    v.MonumentoId,
                    v.UtilizadorId,
                    v.NumeroVisitas
                })
                .ToListAsync();

            return Ok(visitas);
        }

        // GET: api/VisitaApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetVisita(int id)
        {
            var visita = await _context.VisitaMonumento
                .Where(v => v.Id == id)
                .Select(v => new
                {
                    v.Id,
                    v.MonumentoId,
                    v.UtilizadorId,
                    v.NumeroVisitas
                })
                .FirstOrDefaultAsync();

            if (visita == null) return NotFound();

            return Ok(visita);
        }

        // POST: api/VisitaApi
        [HttpPost]
        public async Task<ActionResult<object>> PostVisita([FromBody] VisitaCreateDto dto)
        {
            var visita = new VisitaMonumento
            {
                MonumentoId = dto.MonumentoId,
                UtilizadorId = dto.UtilizadorId,
                NumeroVisitas = 1
            };

            _context.VisitaMonumento.Add(visita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVisita), new { id = visita.Id }, new
            {
                visita.Id,
                visita.MonumentoId,
                visita.UtilizadorId,
                visita.NumeroVisitas
            });
        }

        // PUT: api/VisitaApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVisita(int id, [FromBody] VisitaUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var visita = await _context.VisitaMonumento.FindAsync(id);
            if (visita == null) return NotFound();

            visita.NumeroVisitas = dto.NumeroVisitas < 1 ? 1 : dto.NumeroVisitas;
            _context.Entry(visita).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/VisitaApi/5
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
