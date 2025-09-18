using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class LocalidadeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocalidadeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Localidade
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var localidades = await _context.Localidade
                .Include(l => l.Utilizador)
                .ToListAsync();
            return View(localidades);
        }

        // GET: Localidade/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var localidade = await _context.Localidade
                .Include(l => l.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (localidade == null) return NotFound();

            return View(localidade);
        }

        // GET: Localidade/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Localidade/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Localidade localidade)
        {
            if (!ModelState.IsValid) return View(localidade);

            // Associamos o admin actual como criador (opcional)
            /*var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador != null) localidade.UtilizadorId = utilizador.Id;*/
            
            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            if (utilizador == null)
            {
                TempData["Erro"] = "Não foi possível identificar o utilizador autenticado.";
                return View(localidade);
            }

            // Associar SEMPRE o criador
            localidade.UtilizadorId = utilizador.Id;

            //_context.Add(localidade);
            _context.Localidade.Add(localidade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Localidade/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var localidade = await _context.Localidade
                .Include(l => l.Utilizador)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (localidade == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && localidade.UtilizadorId != utilizador?.Id)
                return Forbid();

            return View(localidade);
        }

        // POST: Localidade/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Localidade localidadeEditada)
        {
            if (!ModelState.IsValid) return View(localidadeEditada);

            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && localidade.UtilizadorId != utilizador?.Id)
                return Forbid();

            localidade.NomeLocalidade = localidadeEditada.NomeLocalidade;

            try
            {
                _context.Update(localidade);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocalidadeExists(localidade.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Localidade/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var localidade = await _context.Localidade
                .Include(l => l.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (localidade == null) return NotFound();
            return View(localidade);
        }

        // POST: Localidade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade != null)
            {
                _context.Localidade.Remove(localidade);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        
        // Helper para sincronizar Identity com tabela Utilizador
        private async Task<Utilizador?> GetOrCreateUtilizadorForCurrentIdentityAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;

            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador != null) return utilizador;

            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                             ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            utilizador = new Utilizador
            {
                Username = username,
                Nome = username,
                Email = emailClaim ?? $"{username}@dummy.local",
                LocalidadeUtilizador = "N/D",
                Password = "IdentityManaged"
            };

            _context.Utilizador.Add(utilizador);
            await _context.SaveChangesAsync();

            return utilizador;
        }

        private bool LocalidadeExists(int id) => _context.Localidade.Any(e => e.Id == id);
    }
}
