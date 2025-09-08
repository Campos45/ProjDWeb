using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using appMonumentos.Models;
using appMonumentos.Models.Account;

namespace WebApplication1.Controllers
{
    public class UtilizadorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UtilizadorController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public IActionResult Index()
        {
            var utilizadores = _context.Utilizador.ToList();
            return View(utilizadores);
        }
        
        // GET: Utilizador/Details/5
        public IActionResult Details(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();
            return View(utilizador);
        }
        
        // GET: Utilizador/Edit/5
        public IActionResult Edit(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

        // POST: Utilizador/Edit/5
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

        // GET: Utilizador/Delete/5
        public IActionResult Delete(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

        // POST: Utilizador/Delete/5
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


        // ============================
        // REGISTO
        // ============================
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var identityUser = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                // Role padrão
                await _userManager.AddToRoleAsync(identityUser, "Normal");

                // Guardar também na tabela Utilizador
                var utilizador = new Utilizador
                {
                    Username = model.Username,
                    Password = model.Password, // 
                    Nome = model.Nome,
                    LocalidadeUtilizador = model.LocalidadeUtilizador,
                    Email = model.Email
                };

                _context.Utilizador.Add(utilizador);
                await _context.SaveChangesAsync();

                // Login automático após registo
                await _signInManager.SignInAsync(identityUser, isPersistent: false);

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ============================
        // LOGIN
        // ============================
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError(string.Empty, "Login inválido.");
            return View(model);
        }

        // ============================
        // LOGOUT
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
