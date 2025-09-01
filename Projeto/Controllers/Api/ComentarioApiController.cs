using Microsoft.AspNetCore.Mvc;         
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;               
using appMonumentos.Models;               

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
        public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios()
        {
            // Consulta à base de dados para obter todos os comentários com as respetivas imagens e utilizadores associados
            return await _context.Comentario
                .Include(c => c.Imagem)    // Inclui os dados da imagem associada ao comentário
                .Include(c => c.Utilizador) // Inclui os dados do utilizador que fez o comentário
                .ToListAsync();            // Executa a consulta e converte para lista
        }

        // GET: api/ComentarioApi/{id}
        // Método que obtém um comentário específico pelo seu ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            // Pesquisa na base de dados o comentário com os dados relacionados
            var comentario = await _context.Comentario
                .Include(c => c.Imagem)
                .Include(c => c.Utilizador)
                .FirstOrDefaultAsync(c => c.Id == id);

            // Se o comentário não existir, retorna 404 Not Found
            if (comentario == null) return NotFound();

            // Cria um objeto anónimo simplificado só com os campos relevantes
            var resultado = new
            {
                comentario.ComentarioTexto,
                comentario.Data,
                comentario.ImagemId,
                UtilizadorNome = comentario.Utilizador?.Nome
            };

            return Ok(resultado);
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
