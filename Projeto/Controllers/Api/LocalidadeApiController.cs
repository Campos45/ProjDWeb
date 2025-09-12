using Microsoft.AspNetCore.Mvc;          
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;               
using appMonumentos.Models;
using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    // Indica que este controlador é usado para API REST
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Define a rota base da API para este controlador: "api/LocalidadeApi"
    [Route("api/[controller]")]
    public class LocalidadeApiController : ControllerBase  
    {
        //base de dados para acesso aos dados
        private readonly ApplicationDbContext _context;

        // Construtor que recebe a base de dados
        public LocalidadeApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Localidade
        // Método que retorna uma lista de todas as localidades
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

        // GET: api/Localidade/{id}
        // Método que obtém uma localidade específica pelo seu ID
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

            if (localidade == null)
            {
                return NotFound();
            }

            return Ok(localidade);
        }

        // POST: api/Localidade
        // Método para adicionar uma nova localidade
        [HttpPost]
        public async Task<ActionResult> PostLocalidade([FromBody] LocalidadeCreateDto dto)
        {
            var utilizador = await _context.Utilizador.FindAsync(dto.UtilizadorId);
            if (utilizador == null)
            {
                return BadRequest("Utilizador não encontrado.");
            }

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



        // PUT: api/Localidade/{id}
        // Método para atualizar uma localidade existente pelo seu ID
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


        // DELETE: api/Localidade/{id}
        // Método para apagar uma localidade pelo seu ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocalidade(int id)
        {
            // Procura a localidade na base de dados pelo ID
            var localidade = await _context.Localidade.FindAsync(id);

            // Se não existir, retorna 404 Not Found
            if (localidade == null) return NotFound();

            // Remove a localidade do contexto
            _context.Localidade.Remove(localidade);

            // Aplica as alterações na base de dados
            await _context.SaveChangesAsync();

            // Retorna resposta HTTP 204 No Content indicando que a operação foi bem-sucedida mas sem conteúdo a devolver
            return NoContent();
        }
    }
}
