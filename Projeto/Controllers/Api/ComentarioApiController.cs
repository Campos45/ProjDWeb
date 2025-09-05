using Microsoft.AspNetCore.Mvc;         
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;               
using appMonumentos.Models;
using appMonumentos.Models.Dto;

namespace WebApplication1.Controllers.Api
{
    // Define a rota base da API para este controlador: "api/ComentarioApi"
    [Route("api/[controller]")]
    [ApiController]   // Indica que este controlador é usado para API REST
    public class ComentarioApiController : ControllerBase  
    {
        // base de dados para aceder aos dados
        private readonly ApplicationDbContext _context;

        // Construtor que recebe a base de dados
        public ComentarioApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ComentarioApi
        // Método que retorna uma lista de todos os comentários
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComentarioDto>>> GetComentarios()
        {
            return await _context.Comentario
                .Include(c => c.Utilizador)
                .Select(c => new ComentarioDto
                {
                    Id = c.Id,
                    ComentarioTexto = c.ComentarioTexto,
                    Data = c.Data,
                    ImagemId = c.ImagemId,
                    UtilizadorId = c.UtilizadorId,
                    NomeAutor = c.Utilizador.Nome
                })
                .ToListAsync();
        }

        // GET: api/ComentarioApi/{id}
        // Método que obtém um comentário específico pelo seu ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ComentarioDto>> GetComentario(int id)
        {
            var comentario = await _context.Comentario
                .Include(c => c.Utilizador)
                .Where(c => c.Id == id)
                .Select(c => new ComentarioDto
                {
                    Id = c.Id,
                    ComentarioTexto = c.ComentarioTexto,
                    Data = c.Data,
                    ImagemId = c.ImagemId,
                    UtilizadorId = c.UtilizadorId,
                    NomeAutor = c.Utilizador.Nome
                })
                .FirstOrDefaultAsync();

            if (comentario == null) return NotFound();

            return comentario;
        }

        // POST: api/ComentarioApi
        // Método para adicionar um novo comentário
        [HttpPost]
        public async Task<ActionResult<Comentario>> PostComentario(Comentario comentario)
        {
            // Define a data do comentário como o momento atual
            comentario.Data = DateTime.Now;

            // Adiciona o novo comentário a base de dados
            _context.Comentario.Add(comentario);

            // Guarda as alterações na base de dados 
            await _context.SaveChangesAsync();

            // Retorna a resposta HTTP 201 (Created) com o local do novo comentário criado e o próprio comentário
            return CreatedAtAction(nameof(GetComentario), new { id = comentario.Id }, comentario);
        }

        // DELETE: api/ComentarioApi/{id}
        // Método para apagar um comentário pelo seu ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            // Procura o comentário na base de dados pelo ID
            var comentario = await _context.Comentario.FindAsync(id);

            // Se não existir, retorna 404 Not Found
            if (comentario == null) return NotFound();

            // Remove o comentário da base de dados
            _context.Comentario.Remove(comentario);

            // Aplica as alterações na base de dados
            await _context.SaveChangesAsync();

            // Retorna resposta HTTP 204 No Content indicando que a operação foi bem-sucedida mas sem conteúdo a devolver
            return NoContent();
        }
    }
}
