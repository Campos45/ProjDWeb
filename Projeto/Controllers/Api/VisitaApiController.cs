using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    // Define que esta classe é um API Controller e usa a rota padrão "api/[controller]"
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class VisitaApiController : ControllerBase
    {
        //base de dados para acesso aos dados
        private readonly ApplicationDbContext _context;

        // Construtor que recebe a base de dados
        public VisitaApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/VisitaApi
        // Retorna a lista de todas as visitas, incluindo os utilizadores e monumentos relacionados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetVisitas()
        {
            var visitas = await _context.VisitaMonumento
                .Select(v => new
                {
                    v.Id,
                    v.MonumentoId,
                    v.UtilizadorId
                })
                .ToListAsync();

            return Ok(visitas);
        }

        // GET: api/VisitaApi/5
        // Retorna uma visita específica pelo seu id
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetVisita(int id)
        {
            var visita = await _context.VisitaMonumento
                .Where(v => v.Id == id)
                .Select(v => new
                {
                    v.Id,
                    v.MonumentoId,
                    v.UtilizadorId
                })
                .FirstOrDefaultAsync();

            if (visita == null)
            {
                return NotFound();
            }

            return Ok(visita);
        }

        // POST: api/VisitaApi
        // Adiciona uma nova visita à base de dados
        [HttpPost]
        public async Task<ActionResult<VisitaCreateDto>> PostVisita([FromBody] VisitaCreateDto dto)
        {
            var visita = new VisitaMonumento
            {
                MonumentoId = dto.MonumentoId,
                UtilizadorId = dto.UtilizadorId
            };

            _context.VisitaMonumento.Add(visita);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVisita), new { id = visita.Id }, dto);
        }
        
        //Put
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVisita(int id, [FromBody] VisitaUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var visita = await _context.VisitaMonumento.FindAsync(id);
            if (visita == null) return NotFound();

            visita.MonumentoId = dto.MonumentoId;
            visita.UtilizadorId = dto.UtilizadorId;

            _context.Entry(visita).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // DELETE: api/VisitaApi/5
        // Remove uma visita pelo seu id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVisita(int id)
        {
            var visita = await _context.VisitaMonumento.FindAsync(id);

            // Se não encontrar, retorna 404 Not Found
            if (visita == null) return NotFound();

            _context.VisitaMonumento.Remove(visita);
            await _context.SaveChangesAsync();

            // Retorna 204 No Content indicando que a eliminação foi bem sucedida
            return NoContent();
        }
    }
}
