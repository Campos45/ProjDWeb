using Microsoft.AspNetCore.Mvc;          
using Microsoft.EntityFrameworkCore;    
using WebApplication1.Data;              
using appMonumentos.Models;               

namespace WebApplication1.Controllers.Api
{
    // Indica que este controlador é usado para API REST
    [ApiController]

    // Define a rota base da API para este controlador: "api/ImagemApi"
    [Route("api/[controller]")]
    public class ImagemApiController : ControllerBase  
    {
        //base de dados para acesso aos dados
        private readonly ApplicationDbContext _context;

        // Construtor que recebe a base de dados
        public ImagemApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ImagemApi
        // Método que retorna uma lista de todas as imagens
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Imagem>>> GetImagens()
        {
            // Consulta à base de dados para obter todas as imagens com as respetivas informações do utilizador e monumento associados
            return await _context.Imagem
                .Include(i => i.Utilizador)  // Inclui os dados do utilizador que inseriu a imagem
                .Include(i => i.Monumento)  // Inclui os dados do monumento associado à imagem
                .ToListAsync();              // Executa a consulta e converte para lista
        }

        // GET: api/ImagemApi/{id}
        // Método que obtém uma imagem específica pelo seu ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Imagem>> GetImagem(int id)
        {
            // Pesquisa na base de dados a imagem com o ID fornecido, incluindo os comentários associados
            var imagem = await _context.Imagem
                .Include(i => i.Comentarios)           // Inclui a coleção de comentários relacionados a esta imagem
                .FirstOrDefaultAsync(i => i.Id == id); // Encontra a primeira imagem com o ID correspondente

            // Se a imagem não existir, retorna um código HTTP 404 (Not Found)
            if (imagem == null) return NotFound();

            // Retorna a imagem encontrada
            return imagem;
        }

        // POST: api/ImagemApi
        // Método para adicionar uma nova imagem
        [HttpPost]
        public async Task<ActionResult<Imagem>> PostImagem(Imagem imagem)
        {
            // Adiciona a nova imagem ao contexto da base de dados
            _context.Imagem.Add(imagem);

            // Guarda as alterações na base de dados 
            await _context.SaveChangesAsync();

            // Retorna a resposta HTTP 201 (Created) com o local da nova imagem criada e a própria imagem
            return CreatedAtAction(nameof(GetImagem), new { id = imagem.Id }, imagem);
        }

        // DELETE: api/ImagemApi/{id}
        // Método para apagar uma imagem pelo seu ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImagem(int id)
        {
            // Procura a imagem na base de dados pelo ID
            var imagem = await _context.Imagem.FindAsync(id);

            // Se não existir, retorna 404 Not Found
            if (imagem == null) return NotFound();

            // Remove a imagem do contexto
            _context.Imagem.Remove(imagem);

            // Aplica as alterações na base de dados
            await _context.SaveChangesAsync();

            // Retorna resposta HTTP 204 No Content indicando que a operação foi bem-sucedida mas sem conteúdo a devolver
            return NoContent();
        }
    }
}
