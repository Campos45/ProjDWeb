using Microsoft.AspNetCore.Mvc;           
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;                
using appMonumentos.Models;                
using System.Collections.Generic;        
using System.Threading.Tasks;
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
        public async Task<ActionResult<Monumento>> PostMonumento(Monumento monumento)
        {
            // Adiciona o novo monumento a base de dados
            _context.Monumento.Add(monumento);

            // Guarda as alterações na base de dados
            await _context.SaveChangesAsync();
            

            // Retorna a resposta HTTP 201 (Created) com o local da nova entidade criada e a própria entidade
            return CreatedAtAction(nameof(GetMonumento), new { id = monumento.Id }, monumento);
        }

        // PUT: api/monumentos/5
        // Método para atualizar um monumento existente pelo seu ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMonumento(int id, Monumento monumento)
        {
            // Se o ID do URL for diferente do ID do objeto monumento enviado, retorna 400 Bad Request
            if (id != monumento.Id)
            {
                return BadRequest();
            }

            // Marca a entidade como modificada para que seja atualizada na base de dados
            _context.Entry(monumento).State = EntityState.Modified;

            try
            {
                // Tenta guardar as alterações na base de dados 
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se o monumento não existir na base de dados, retorna 404 Not Found
                if (!_context.Monumento.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                // Se for outro erro de concorrência, propaga a exceção
                throw;
            }

            // Retorna 204 No Content indicando que a atualização foi bem-sucedida, sem conteúdo a devolver
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
