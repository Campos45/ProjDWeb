using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using appMonumentos.Models;
using appMonumentos.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Controllers
{
    /// Controlador responsável pela gestão de utilizadores (CRUD + Autenticação)
    public class UtilizadorController : Controller
    {
        private readonly ApplicationDbContext _context;               // Acesso à BD
        private readonly UserManager<IdentityUser> _userManager;      // Gestão de utilizadores (Identity)
        private readonly SignInManager<IdentityUser> _signInManager;  // Gestão de login/logout

        public UtilizadorController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ======================
        // LISTAR UTILIZADORES (apenas Admins)
        // ======================
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var utilizadores = _context.Utilizador
                .Include(u => u.VisitasAosMonumentos)
                .ThenInclude(v => v.Monumento)
                .ToList();

            return View(utilizadores);
        }

        /// Mostrar detalhes de um utilizador
        public IActionResult Details(int id)
        {
            var utilizador = _context.Utilizador
                .Include(u => u.VisitasAosMonumentos)
                .ThenInclude(v => v.Monumento)
                .FirstOrDefault(u => u.Id == id);

            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

        /// GET: Editar utilizador
        public IActionResult Edit(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

        /// POST: Editar utilizador
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Utilizador utilizador)
        {
            if (id != utilizador.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                _context.Update(utilizador);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(utilizador);
        }

        /// GET: Apagar utilizador
        public IActionResult Delete(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

        /// POST: Confirmar eliminação
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();

            _context.Utilizador.Remove(utilizador);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ======================
        // REGISTO DE UTILIZADOR
        // ======================
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Criar IdentityUser
            var identityUser = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                // Atribuir role por defeito
                await _userManager.AddToRoleAsync(identityUser, "Normal");

                // Criar utilizador na tabela personalizada
                var utilizador = new Utilizador
                {
                    Username = model.Username,
                    Password = model.Password, // ⚠️ gravado só para consistência com o modelo, não usado em login
                    Nome = model.Nome,
                    LocalidadeUtilizador = model.LocalidadeUtilizador,
                    Email = model.Email
                };

                _context.Utilizador.Add(utilizador);
                await _context.SaveChangesAsync();

                // Login imediato após registo
                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Se falhar, mostrar erros
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ======================
        // LOGIN / LOGOUT
        // ======================
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded) return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "Login inválido.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
