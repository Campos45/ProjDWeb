using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    public class UtilizadorController : Controller
    {
        private readonly ApplicationDbContext _context; // Contexto da BD
        private readonly UserManager<IdentityUser> _userManager; // Gestão de utilizadores Identity
        private readonly SignInManager<IdentityUser> _signInManager; // Gestão de login Identity

        // Construtor para injetar dependências
        public UtilizadorController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Listar todos os utilizadores - só para admins
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizador.ToListAsync());
        }

        // GET: Mostrar página para criar utilizador (registo)
        public IActionResult Create()
        {
            return View();
        }

        // POST: Criar utilizador na BD e no Identity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Nome,LocalidadeUtilizador,Email")] Utilizador utilizador)
        {
            // Verificar se o modelo é válido
            if (!ModelState.IsValid) return View(utilizador);

            // Criar novo IdentityUser para autenticação
            var identityUser = new IdentityUser
            {
                UserName = utilizador.Username,
                Email = utilizador.Email
            };

            // Criar utilizador no Identity com password
            var result = await _userManager.CreateAsync(identityUser, utilizador.Password);
            if (!result.Succeeded)
            {
                // Se houver erros, adicioná-los ao ModelState e mostrar vista novamente
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(utilizador);
            }

            // Atribuir role padrão "Normal" ao novo utilizador
            await _userManager.AddToRoleAsync(identityUser, "Normal");

            // Guardar dados adicionais na tabela Utilizador
            utilizador.Username = identityUser.UserName;
            _context.Add(utilizador);
            await _context.SaveChangesAsync();

            // Após registo, redirecionar para o login
            return RedirectToAction("Login");
        }

        // GET: Mostrar página de login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Autenticar utilizador com username e password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (!ModelState.IsValid) return View();

            // Tentar fazer login com as credenciais
            var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Se sucesso, redirecionar para página principal
                return RedirectToAction("Index", "Home");
            }

            // Se falhar login, mostrar erro
            ModelState.AddModelError(string.Empty, "Login inválido. Verifique o utilizador e a password.");
            return View();
        }

        // GET: Ver detalhes de um utilizador (autenticado)
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Obter utilizador da BD
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null) return NotFound();

            return View(utilizador);
        }

        // GET: Mostrar página para editar utilizador (autenticado)
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null) return NotFound();

            return View(utilizador);
        }

        // POST: Atualizar dados do utilizador na BD
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Nome,LocalidadeUtilizador,Email")] Utilizador utilizador)
        {
            if (id != utilizador.Id) return NotFound();

            if (!ModelState.IsValid) return View(utilizador);

            try
            {
                // Atualizar registo na BD
                _context.Update(utilizador);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Se não existir, retorna erro
                if (!UtilizadorExists(utilizador.Id)) return NotFound();
                throw;
            }

            // Após edição, redirecionar para lista de utilizadores
            return RedirectToAction(nameof(Index));
        }

        // GET: Mostrar página para confirmar eliminação - só admins
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null) return NotFound();

            return View(utilizador);
        }

        // POST: Eliminar utilizador da BD - só admins
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador != null)
                _context.Utilizador.Remove(utilizador);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para verificar se utilizador existe
        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }
    }
}
