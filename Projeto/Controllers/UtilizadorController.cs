using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using appMonumentos.Models;
using appMonumentos.Models.Account;
using Microsoft.EntityFrameworkCore;

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
            var utilizadores = _context.Utilizador
                .Include(u => u.VisitasAosMonumentos)
                .ThenInclude(v => v.Monumento)
                .ToList();

            return View(utilizadores);
        }

        public IActionResult Details(int id)
        {
            var utilizador = _context.Utilizador
                .Include(u => u.VisitasAosMonumentos)
                .ThenInclude(v => v.Monumento)
                .FirstOrDefault(u => u.Id == id);

            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

        public IActionResult Edit(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

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

        public IActionResult Delete(int id)
        {
            var utilizador = _context.Utilizador.FirstOrDefault(u => u.Id == id);
            if (utilizador == null) return NotFound();
            return View(utilizador);
        }

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

        // REGISTO
        public IActionResult Register() => View();

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
                await _userManager.AddToRoleAsync(identityUser, "Normal");

                var utilizador = new Utilizador
                {
                    Username = model.Username,
                    Password = model.Password,
                    Nome = model.Nome,
                    LocalidadeUtilizador = model.LocalidadeUtilizador,
                    Email = model.Email
                };

                _context.Utilizador.Add(utilizador);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(identityUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // LOGIN
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
