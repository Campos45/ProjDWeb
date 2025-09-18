using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers.Api
{
    /// API responsável pela gestão de imagens (listar, consultar e apagar)
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class ImagemApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ImagemApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ImagemApi
        /// Lista todas as imagens registadas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetImagens()
        {
            var imagens = await _context.Imagem
                .Select(i => new
                {
                    i.Id,
                    Caminho = "/imagens/" + i.NomeImagem, // devolve o caminho completo
                    i.MonumentoId,
                    i.UtilizadorId,
                    i.IsPrincipal
                })
                .ToListAsync();

            return Ok(imagens);
        }

        // GET: api/ImagemApi/{id}
        /// Retorna os detalhes de uma imagem específica
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetImagem(int id)
        {
            var imagem = await _context.Imagem
                .Where(i => i.Id == id)
                .Select(i => new
                {
                    i.Id,
                    Caminho = "/imagens/" + i.NomeImagem,
                    i.MonumentoId,
                    i.UtilizadorId,
                    i.IsPrincipal
                })
                .FirstOrDefaultAsync();

            if (imagem == null) return NotFound();

            return Ok(imagem);
        }

        // DELETE: api/ImagemApi/{id}
        /// Apaga uma imagem pelo ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImagem(int id)
        {
            var imagem = await _context.Imagem.FindAsync(id);
            if (imagem == null) return NotFound();

            _context.Imagem.Remove(imagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
