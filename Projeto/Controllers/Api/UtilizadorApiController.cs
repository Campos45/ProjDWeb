using Microsoft.AspNetCore.Mvc;           
using Microsoft.EntityFrameworkCore;      
using WebApplication1.Data;                
using appMonumentos.Models;
using appMonumentos.Models.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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
        private readonly UserManager<IdentityUser> _userManager;

        // Construtor que recebe a base de dados
        public UtilizadorApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
        public async Task<IActionResult> PostUtilizador(UtilizadorCreateDto dto)
        {
            // 1. Criar IdentityUser
            var identityUser = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // 2. Criar Utilizador extra (tua tabela)
            var utilizador = new Utilizador
            {
                Username = dto.Username,
                Password = dto.Password, // 
                Nome = dto.Nome,
                LocalidadeUtilizador = dto.LocalidadeUtilizador,
                Email = dto.Email
            };

            _context.Utilizador.Add(utilizador);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Utilizador criado com sucesso",
                UserId = identityUser.Id,
                ExtraId = utilizador.Id
            });
        }




        // PUT: api/UtilizadorApi/5
        // Método para atualizar um utilizador existente
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUtilizador(int id, [FromBody] UtilizadorUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest();

            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null) return NotFound();

            utilizador.Username = dto.Username;
            utilizador.Nome = dto.Nome;
            utilizador.LocalidadeUtilizador = dto.LocalidadeUtilizador;
            utilizador.Email = dto.Email;

            _context.Entry(utilizador).State = EntityState.Modified;
            await _context.SaveChangesAsync();

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
