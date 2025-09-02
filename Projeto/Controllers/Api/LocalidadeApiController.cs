using Microsoft.AspNetCore.Mvc;          
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;               
using appMonumentos.Models;               

namespace WebApplication1.Controllers.Api
{
    // Indica que este controlador é usado para API REST
    [ApiController]

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
        public async Task<ActionResult<Localidade>> PostLocalidade(Localidade localidade)
        {
            // Adiciona a nova localidade ao contexto da base de dados
            _context.Localidade.Add(localidade);

            // Guarda as alterações na base de dados 
            await _context.SaveChangesAsync();

            // Retorna a resposta HTTP 201 (Created) com o local da nova localidade criada e a própria localidade
            return CreatedAtAction(nameof(GetLocalidade), new { id = localidade.Id }, localidade);
        }

        // PUT: api/Localidade/{id}
        // Método para atualizar uma localidade existente pelo seu ID
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLocalidade(int id, Localidade localidade)
        {
            // Se o ID do URL for diferente do ID do objeto localidade enviado, retorna 400 Bad Request
            if (id != localidade.Id)
                return BadRequest();

            // Marca a entidade como modificada para que seja atualizada na base de dados
            _context.Entry(localidade).State = EntityState.Modified;

            try
            {
                // Tenta guardar as alterações na base de dados
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se a localidade não existir na base de dados, retorna 404 Not Found
                if (!_context.Localidade.Any(e => e.Id == id))
                    return NotFound();
                else
                    // Se for outro erro, propaga a exceção
                    throw;
            }

            // Retorna 204 No Content indicando que a atualização foi bem-sucedida, sem conteúdo a devolver
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
