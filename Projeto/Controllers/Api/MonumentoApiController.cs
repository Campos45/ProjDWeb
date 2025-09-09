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
    // Indica que este controlador é usado para API REST
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Define a rota base da API para este controlador: "api/MonumentoApi"
    [Route("api/[controller]")]
    public class MonumentoApiController : ControllerBase  
    {
        //base de dados para acesso aos dados
        private readonly ApplicationDbContext _context;

        // Construtor que recebe a base de dados
        public MonumentoApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/monumentos
        // Método que retorna uma lista de todos os monumentos
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

        // GET: api/monumentos/5
        // Método que obtém um monumento específico pelo seu ID
        // GET: api/MonumentoApi/{id}
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

            if (monumento == null)
            {
                return NotFound();
            }

            return Ok(monumento);
        }


        // POST: api/monumentos
        // Método para adicionar um novo monumento
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


        // PUT: api/monumentos/5
        // Método para atualizar um monumento existente pelo seu ID
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


        // DELETE: api/monumentos/5
        // Método para apagar um monumento pelo seu ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonumento(int id)
        {
            // Procura o monumento na base de dados pelo ID
            var monumento = await _context.Monumento.FindAsync(id);

            // Se não existir, retorna 404 Not Found
            if (monumento == null)
            {
                return NotFound();
            }

            // Remove o monumento do contexto
            _context.Monumento.Remove(monumento);

            // Aplica as alterações na base de dados
            await _context.SaveChangesAsync();

            // Retorna resposta HTTP 204 No Content indicando que a operação foi bem-sucedida mas sem conteúdo a devolver
            return NoContent();
        }
    }
}
