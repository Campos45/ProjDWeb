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
    /// API responsável pela gestão dos utilizadores
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UtilizadorApiController : ControllerBase  
    {
        private readonly ApplicationDbContext _context;  // Contexto da base de dados
        private readonly UserManager<IdentityUser> _userManager; // Gestão de utilizadores do Identity

        // Construtor que recebe o contexto e o gestor de utilizadores
        public UtilizadorApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/UtilizadorApi
        /// Retorna a lista de todos os utilizadores (apenas dados essenciais)
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

        // GET: api/UtilizadorApi/{id}
        /// Retorna os dados de um utilizador específico
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

            if (utilizador == null) return NotFound();

            return Ok(utilizador);
        }

        // POST: api/UtilizadorApi
        /// Cria um novo utilizador (tanto no Identity como na tabela personalizada)
        [HttpPost]
        public async Task<IActionResult> PostUtilizador(UtilizadorCreateDto dto)
        {
            // 1. Criação do utilizador no Identity
            var identityUser = new IdentityUser
            {
                UserName = dto.Username,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            // 2. Inserção na tabela  "Utilizador"
            var utilizador = new Utilizador
            {
                Username = dto.Username,
                Password = dto.Password, 
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

        // PUT: api/UtilizadorApi/{id}
        /// Atualiza um utilizador existente na tabela personalizada
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

        // DELETE: api/UtilizadorApi/{id}
        /// Remove um utilizador da base de dados
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUtilizador(int id)
        {
            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null) return NotFound();

            _context.Utilizador.Remove(utilizador);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
