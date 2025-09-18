using Microsoft.AspNetCore.Mvc;          
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;               
using appMonumentos.Models;
using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    /// API responsável pela gestão das localidades
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class LocalidadeApiController : ControllerBase  
    {
        private readonly ApplicationDbContext _context;

        public LocalidadeApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/LocalidadeApi
        ///Lista todas as localidades
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLocalidades()
        {
            var localidades = await _context.Localidade
                .Select(l => new
                {
                    l.Id,
                    l.NomeLocalidade
                })
                .ToListAsync();

            return Ok(localidades);
        }

        // GET: api/LocalidadeApi/{id}
        /// Devolve os detalhes de uma localidade específica
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetLocalidade(int id)
        {
            var localidade = await _context.Localidade
                .Where(l => l.Id == id)
                .Select(l => new
                {
                    l.Id,
                    l.NomeLocalidade
                })
                .FirstOrDefaultAsync();

            if (localidade == null) return NotFound();

            return Ok(localidade);
        }

        // POST: api/LocalidadeApi
        /// Cria uma nova localidade
        [HttpPost]
        public async Task<ActionResult> PostLocalidade([FromBody] LocalidadeCreateDto dto)
        {
            var utilizador = await _context.Utilizador.FindAsync(dto.UtilizadorId);
            if (utilizador == null) return BadRequest("Utilizador não encontrado.");

            var localidade = new Localidade
            {
                NomeLocalidade = dto.NomeLocalidade,
                UtilizadorId = dto.UtilizadorId
            };

            _context.Localidade.Add(localidade);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLocalidade), new { id = localidade.Id }, new
            {
                localidade.Id,
                localidade.NomeLocalidade,
                localidade.UtilizadorId
            });
        }

        // PUT: api/LocalidadeApi/{id}
        /// Atualiza os dados de uma localidade existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocalidade(int id, [FromBody] LocalidadeUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade == null) return NotFound();

            localidade.NomeLocalidade = dto.NomeLocalidade;

            _context.Entry(localidade).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/LocalidadeApi/{id}
        /// Apaga uma localidade
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
