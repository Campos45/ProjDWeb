using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Mvc;         
using Microsoft.EntityFrameworkCore;     
using WebApplication1.Data;               
using appMonumentos.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    /// API REST responsável por gerir comentários nas imagens
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ComentarioApiController : ControllerBase  
    {
        private readonly ApplicationDbContext _context; // Contexto da base de dados

        public ComentarioApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ComentarioApi
        /// Devolve a lista de todos os comentários com informação adicional do autor
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
        /// Devolve os detalhes de um comentário específico
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
        /// Cria um novo comentário
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

            var responseDto = new ComentarioCreateDto
            {
                ComentarioTexto = comentario.ComentarioTexto,
                Data = comentario.Data,
                ImagemId = comentario.ImagemId,
                UtilizadorId = comentario.UtilizadorId
            };

            return CreatedAtAction(nameof(GetComentario), new { id = comentario.Id }, responseDto);
        }

        // PUT: api/ComentarioApi/{id}
        /// Atualiza um comentário existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComentario(int id, [FromBody] ComentarioUpdateDto dto)
        {
            if (id != dto.Id)
                return BadRequest();

            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario == null)
                return NotFound();

            comentario.ComentarioTexto = dto.ComentarioTexto;
            comentario.Data = dto.Data;

            _context.Entry(comentario).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ComentarioApi/{id}
        /// Apaga um comentário pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComentario(int id)
        {
            var comentario = await _context.Comentario.FindAsync(id);
            if (comentario == null) return NotFound();

            _context.Comentario.Remove(comentario);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
