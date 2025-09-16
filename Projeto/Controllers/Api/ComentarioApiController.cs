using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Mvc;         
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;               
using appMonumentos.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    // Define a rota base da API para este controlador: "api/ComentarioApi"
    [Route("api/[controller]")]
    [ApiController]   // Indica que este controlador é usado para API REST
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

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
        public async Task<ActionResult<ComentarioCreateDto>> PostComentario([FromBody] ComentarioCreateDto dto)
        {
            var comentario = new Comentario
            {
                ComentarioTexto = dto.ComentarioTexto,
                Data = dto.Data,
                ImagemId = dto.ImagemId,
                UtilizadorId = dto.UtilizadorId
            };

            _context.Comentario.Add(comentario);
            await _context.SaveChangesAsync();

            // resposta minimal, apenas com o que interessa
            
            var responseDto = new ComentarioCreateDto
            {
                ComentarioTexto = comentario.ComentarioTexto,
                Data = comentario.Data,
                ImagemId = comentario.ImagemId,
                UtilizadorId = comentario.UtilizadorId
            };

            return CreatedAtAction(nameof(GetComentario), new { id = comentario.Id }, responseDto);
        }


        // PUT: api/ComentarioApi
        // Método para editar um comentário
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComentario(int id, [FromBody] ComentarioUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario == null)
                return NotFound();

            // Atualizar apenas os campos permitidos
            comentario.ComentarioTexto = dto.ComentarioTexto;
            comentario.Data = dto.Data;

            _context.Entry(comentario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
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
