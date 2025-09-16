using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication1.Controllers
{
    public class MonumentoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonumentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Monumento
        public async Task<IActionResult> Index()
        {
            var monumentos = await _context.Monumento
                .Include(m => m.Utilizador)
                .Include(m => m.Imagens)
                .Include(m => m.VisitasMonumento)
                .ToListAsync();

            return View(monumentos);
        }

        // GET: Monumento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Utilizador)
                .Include(m => m.Imagens).ThenInclude(i => i.Comentarios).ThenInclude(c => c.Utilizador)
                .Include(m => m.VisitasMonumento).ThenInclude(v => v.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null) return NotFound();

            return View(monumento);
        }

        // GET: Monumento/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade");
            return View();
        }

        // POST: Monumento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Monumento monumento)
        {
            if (ModelState.IsValid)
            {
                var username = User.Identity?.Name;
                var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);

                if (utilizador != null)
                {
                    monumento.UtilizadorId = utilizador.Id;
                }

                _context.Add(monumento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
            return View(monumento);
        }

        // GET: Monumento/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            // carregar dropdown
            ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);

            return View(monumento);
        }


        // POST: Monumento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, Monumento monumento)
        {
            if (id != monumento.Id) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
                return View(monumento);
            }


            try
            {
                _context.Update(monumento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Monumento.Any(e => e.Id == monumento.Id))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Monumento/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            return View(monumento);
        }

        // POST: Monumento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            _context.Monumento.Remove(monumento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Monumento/MarcarVisita
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MarcarVisita(int id)
        {
            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita == null)
            {
                visita = new VisitaMonumento
                {
                    MonumentoId = id,
                    UtilizadorId = utilizador.Id,
                    NumeroVisitas = 1
                };
                _context.VisitaMonumento.Add(visita);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }

        // POST: Monumento/IncrementarVisita
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> IncrementarVisita(int id)
        {
            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita != null)
            {
                if (visita.NumeroVisitas <= 0) visita.NumeroVisitas = 0;
                visita.NumeroVisitas++;
                _context.VisitaMonumento.Update(visita);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }

        // POST: Monumento/DecrementarVisita
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DecrementarVisita(int id)
        {
            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita != null && visita.NumeroVisitas > 1)
            {
                visita.NumeroVisitas--;
                _context.VisitaMonumento.Update(visita);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id });
        }
        
        //Upload imagens
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImagem(int monumentoId, IFormFile imagem)
        {
            if (imagem == null || imagem.Length == 0)
            {
                TempData["Erro"] = "Nenhuma imagem selecionada.";
                return RedirectToAction("Details", new { id = monumentoId });
            }

            var fileName = Path.GetFileName(imagem.FileName);
            var filePath = Path.Combine("wwwroot/imagens", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);

            var novaImagem = new Imagem
            {
                NomeImagem = fileName,
                MonumentoId = monumentoId,
                UtilizadorId = utilizador.Id,
                IsPrincipal = false
            };

            _context.Imagem.Add(novaImagem);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Imagem carregada com sucesso.";
            return RedirectToAction("Details", new { id = monumentoId });
        }
        
        //apagar imagens
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteImagem(int id)
        {
            var imagem = await _context.Imagem
                .Include(i => i.Monumento)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (imagem == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            // Apenas o dono da imagem ou Admin pode apagar
            if (!isAdmin && imagem.UtilizadorId != utilizador?.Id)
                return Forbid();

            // Apagar ficheiro físico
            var filePath = Path.Combine("wwwroot/imagens", imagem.NomeImagem);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Apagar registo da BD
            _context.Imagem.Remove(imagem);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Imagem apagada com sucesso.";

            return RedirectToAction("Details", new { id = imagem.MonumentoId });
        }


    }
}
