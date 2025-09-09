using Microsoft.AspNetCore.Mvc;           
using Microsoft.EntityFrameworkCore;      
using WebApplication1.Data;                
using appMonumentos.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    // Indica que este controlador é um API controller
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    // Define a rota base do controlador (ex: "api/UtilizadorApi")
    [Route("api/[controller]")]
    public class UtilizadorApiController : ControllerBase  
    {
        //base de dados para acesso aos dados
        private readonly ApplicationDbContext _context;

        // Construtor que recebe a base de dados
        public UtilizadorApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/UtilizadorApi
        // Método que retorna a lista de todos os utilizadores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUtilizadores()
        {
            var utilizadores = await _context.Utilizador
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Nome,
                    u.LocalidadeUtilizador,
                    u.Email
                })
                .ToListAsync();

            return Ok(utilizadores);
        }

        // GET: api/UtilizadorApi/5
        // Método que retorna um utilizador específico pelo seu ID
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUtilizador(int id)
        {
            var utilizador = await _context.Utilizador
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Nome,
                    u.LocalidadeUtilizador,
                    u.Email
                })
                .FirstOrDefaultAsync();

            if (utilizador == null)
            {
                return NotFound();
            }

            return Ok(utilizador);
        }


        // POST: api/UtilizadorApi
        // Método para criar um novo utilizador
        [HttpPost]
        public async Task<ActionResult<Utilizador>> PostUtilizador(Utilizador utilizador)
        {
            // Adiciona o novo utilizador ao contexto
            _context.Utilizador.Add(utilizador);

            // Guarda as alterações na base de dados
            await _context.SaveChangesAsync();

            // Retorna 201 Created, com a rota para obter o novo utilizador criado
            return CreatedAtAction(nameof(GetUtilizador), new { id = utilizador.Id }, utilizador);
        }

        // PUT: api/UtilizadorApi/5
        // Método para atualizar um utilizador existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilizador(int id, Utilizador utilizador)
        {
            // Se o ID do URL for diferente do ID do utilizador recebido, retorna 400 Bad Request
            if (id != utilizador.Id) return BadRequest();

            // Marca a entidade como modificada para atualizar na base de dados
            _context.Entry(utilizador).State = EntityState.Modified;

            // Guarda as alterações
            await _context.SaveChangesAsync();

            // Retorna 204 No Content indicando que a atualização ocorreu com sucesso
            return NoContent();
        }

        // DELETE: api/UtilizadorApi/5
        // Método para apagar um utilizador pelo seu ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilizador(int id)
        {
            // Procura o utilizador pelo ID
            var utilizador = await _context.Utilizador.FindAsync(id);

            // Se não encontrado, retorna 404 Not Found
            if (utilizador == null) return NotFound();

            // Remove o utilizador
            _context.Utilizador.Remove(utilizador);

            // Aplica as alterações na base de dados
            await _context.SaveChangesAsync();

            // Retorna 204 No Content indicando que o utilizador foi apagado com sucesso
            return NoContent();
        }
    }
}
