using Microsoft.AspNetCore.Mvc;           
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;                
using appMonumentos.Models;                
using System.Collections.Generic;        
using System.Threading.Tasks;
using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.API
{
    /// API responsável pela gestão de monumentos
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class MonumentoApiController : ControllerBase  
    {
        private readonly ApplicationDbContext _context;

        public MonumentoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MonumentoApi
        /// Lista todos os monumentos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetMonumentos()
        {
            var monumentos = await _context.Monumento
                .Select(m => new
                {
                    m.Id,
                    m.Designacao,
                    m.Endereco,
                    m.Coordenadas,
                    m.EpocaConstrucao,
                    m.Descricao,
                    m.UtilizadorId,
                    m.LocalidadeId
                })
                .ToListAsync();

            return Ok(monumentos);
        }

        // GET: api/MonumentoApi/{id}
        /// Retorna um monumento específico
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetMonumento(int id)
        {
            var monumento = await _context.Monumento
                .Where(m => m.Id == id)
                .Select(m => new
                {
                    m.Id,
                    m.Designacao,
                    m.Endereco,
                    m.Coordenadas,
                    m.EpocaConstrucao,
                    m.Descricao,
                    m.UtilizadorId,
                    m.LocalidadeId
                })
                .FirstOrDefaultAsync();

            if (monumento == null) return NotFound();

            return Ok(monumento);
        }

        // POST: api/MonumentoApi
        /// Cria um novo monumento
        [HttpPost]
        public async Task<ActionResult<MonumentoCreateDto>> PostMonumento([FromBody] MonumentoCreateDto dto)
        {
            var monumento = new Monumento
            {
                Designacao = dto.Designacao,
                Endereco = dto.Endereco,
                Coordenadas = dto.Coordenadas,
                EpocaConstrucao = dto.EpocaConstrucao,
                Descricao = dto.Descricao,
                UtilizadorId = dto.UtilizadorId,
                LocalidadeId = dto.LocalidadeId
            };

            _context.Monumento.Add(monumento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMonumento), new { id = monumento.Id }, dto);
        }

        // PUT: api/MonumentoApi/{id}
        /// Atualiza um monumento existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMonumento(int id, [FromBody] MonumentoUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            monumento.Designacao = dto.Designacao;
            monumento.Endereco = dto.Endereco;
            monumento.Coordenadas = dto.Coordenadas;
            monumento.EpocaConstrucao = dto.EpocaConstrucao;
            monumento.Descricao = dto.Descricao;
            monumento.UtilizadorId = dto.UtilizadorId;
            monumento.LocalidadeId = dto.LocalidadeId;

            _context.Entry(monumento).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/MonumentoApi/{id}
        /// Apaga um monumento
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonumento(int id)
        {
            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            _context.Monumento.Remove(monumento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
