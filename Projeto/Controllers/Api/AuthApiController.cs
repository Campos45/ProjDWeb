using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers.Api
{
    // Controlador responsável pela autenticação e emissão de tokens JWT
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;   // Serviço de gestão de utilizadores
        private readonly SignInManager<IdentityUser> _signInManager; // Serviço para login e validação de credenciais
        private readonly IConfiguration _configuration; // Permite aceder as configurações da aplicação 

        public AuthApiController(UserManager<IdentityUser> userManager,
                                 SignInManager<IdentityUser> signInManager,
                                 IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // Classe auxiliar com os dados necessários para login
        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        // POST: api/AuthApi/login
        // Este método recebe credenciais e devolve um token JWT válido
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Verifica se os dados foram enviados corretamente
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username e password obrigatórios.");

            // Procura o utilizador pelo nome de utilizador
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return Unauthorized("Credenciais inválidas.");

            // Valida a password do utilizador
            var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!check.Succeeded)
                return Unauthorized("Credenciais inválidas.");

            // Criação das claims 
            var userRoles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // Nome de utilizador
                new Claim(ClaimTypes.NameIdentifier, user.Id), // ID do utilizador
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Identificador único do token
            };
            foreach (var r in userRoles) claims.Add(new Claim(ClaimTypes.Role, r));

            // Criação da chave de encriptação para assinar o token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Geração do token JWT
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8), // Token válido por 8h
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Retorna o token e a data de expiração
            return Ok(new { token = tokenString, expires = token.ValidTo });
        }
    }
}
