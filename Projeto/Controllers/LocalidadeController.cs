using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    // Controlador responsável por gerir as localidades (CRUD)
    public class LocalidadeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocalidadeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Localidade
        public async Task<IActionResult> Index()
        {
            var localidades = await _context.Localidade
                .Include(l => l.Utilizador)
                .ToListAsync();
            return View(localidades);
        }

        // GET: Localidade/Details/5
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
        public async Task<IActionResult> Create([Bind("Id,NomeLocalidade")] Localidade localidade)
        {
            if (!ModelState.IsValid) return View(localidade);

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == username);

            if (utilizador == null) return Unauthorized();

            localidade.UtilizadorId = utilizador.Id;

            _context.Add(localidade);
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
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && localidade.UtilizadorId != utilizador?.Id)
                return Forbid();

            return View(localidade);
        }

        // POST: Localidade/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeLocalidade")] Localidade localidadeEditada)
        {
            if (!ModelState.IsValid) return View(localidadeEditada);

            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && localidade.UtilizadorId != utilizador?.Id)
                return Forbid();

            // Só atualiza o NomeLocalidade
            localidade.NomeLocalidade = localidadeEditada.NomeLocalidade;

            try
            {
                _context.Update(localidade);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LocalidadeExists(localidade.Id)) return NotFound();
                else throw;
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

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && localidade.UtilizadorId != utilizador?.Id)
                return Forbid();

            return View(localidade);
        }

        // POST: Localidade/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && localidade.UtilizadorId != utilizador?.Id)
                return Forbid();

            _context.Localidade.Remove(localidade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocalidadeExists(int id)
        {
            return _context.Localidade.Any(e => e.Id == id);
        }
    }
}
