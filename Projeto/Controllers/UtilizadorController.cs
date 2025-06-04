using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

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

        // GET: Utilizador
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizador.ToListAsync());
        }

        // GET: Utilizador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilizador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password,Nome,LocalidadeUtilizador,Email")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                var identityUser = new IdentityUser
                {
                    UserName = utilizador.Username,
                    Email = utilizador.Email
                };

                var result = await _userManager.CreateAsync(identityUser, utilizador.Password);

                if (result.Succeeded)
                {
                    // Só guarda no modelo Utilizador depois de criado o IdentityUser com sucesso
                    _context.Add(utilizador);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(utilizador);
        }

        // GET: Utilizador/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Utilizador/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(username, password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Login inválido. Verifique o utilizador e a password.");
            }

            return View();
        }

        // Outros métodos do CRUD...

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);

            if (utilizador == null)
                return NotFound();

            return View(utilizador);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null)
                return NotFound();

            return View(utilizador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password,Nome,LocalidadeUtilizador,Email")] Utilizador utilizador)
        {
            if (id != utilizador.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utilizador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(utilizador.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(utilizador);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.Id == id);

            if (utilizador == null)
                return NotFound();

            return View(utilizador);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador != null)
                _context.Utilizador.Remove(utilizador);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.Id == id);
        }
    }
}
