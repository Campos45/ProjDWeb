using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

namespace WebApplication1.Controllers
{
    /// Controlador responsável pela gestão dos monumentos no site
    [Authorize]
    public class MonumentoController : Controller
    {
        private readonly ApplicationDbContext _context; // Contexto da BD
        private readonly IWebHostEnvironment _env;      // Ambiente web para manipular ficheiros

        public MonumentoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// Helper: garante que o utilizador autenticado no Identity tem registo na tabela Utilizador.
        private async Task<Utilizador?> GetOrCreateUtilizadorForCurrentIdentityAsync()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username)) return null;

            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador != null) return utilizador;

            // Caso não exista, cria-se um utilizador mínimo
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                             ?? User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;

            utilizador = new Utilizador
            {
                Username = username,
                Nome = username,
                Email = emailClaim ?? $"{username}@dummy.local",
                LocalidadeUtilizador = "N/D",
                Password = "IdentityManaged" // valor fictício só para satisfazer restrição NOT NULL
            };

            _context.Utilizador.Add(utilizador);
            await _context.SaveChangesAsync();
            return utilizador;
        }

        // GET: Monumento
        /// Mostra lista de todos os monumentos com imagens, localidades e visitas
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var monumentos = await _context.Monumento
                .Include(m => m.Utilizador)
                .Include(m => m.Localidade)
                .Include(m => m.Imagens)
                .Include(m => m.VisitasMonumento).ThenInclude(v => v.Utilizador)
                .ToListAsync();

            return View(monumentos);
        }

        // GET: Monumento/Details/5
        /// Mostra detalhes de um monumento específico
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Utilizador)
                .Include(m => m.Localidade)
                .Include(m => m.Imagens).ThenInclude(i => i.Comentarios).ThenInclude(c => c.Utilizador)
                .Include(m => m.VisitasMonumento).ThenInclude(v => v.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null) return NotFound();

            return View(monumento);
        }

        /// GET: Criar novo monumento
        public IActionResult Create()
        {
            ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade");
            ViewBag.CurrentUserName = User.Identity?.Name ?? "";
            return View();
        }

        /// POST: Criar novo monumento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Monumento monumento)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
                ViewBag.CurrentUserName = User.Identity?.Name ?? "";
                return View(monumento);
            }

            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            if (utilizador != null) monumento.UtilizadorId = utilizador.Id;

            // Inicializa coleções para evitar null
            monumento.Imagens ??= new List<Imagem>();
            monumento.VisitasMonumento ??= new List<VisitaMonumento>();

            _context.Add(monumento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        /// GET: Editar monumento
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null) return NotFound();

            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
            return View(monumento);
        }

        /// POST: Editar monumento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Monumento monumento)
        {
            if (id != monumento.Id) return NotFound();

            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            var isAdmin = User.IsInRole("Admin");

            var original = await _context.Monumento.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (original == null) return NotFound();

            if (!isAdmin && original.UtilizadorId != utilizador?.Id)
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewBag.LocalidadeId = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
                return View(monumento);
            }

            try
            {
                // Mantém o dono original
                monumento.UtilizadorId = original.UtilizadorId;

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
        
        /// GET: Apagar monumento
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null) return NotFound();

            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            return View(monumento);
        }

        /// POST: Confirmar remoção do monumento
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            _context.Monumento.Remove(monumento);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ======================
        // UPLOAD E GESTÃO DE IMAGENS
        // ======================

        /// POST: Upload de imagem de monumento
        [HttpPost]
        public async Task<IActionResult> UploadImagem(int monumentoId, IFormFile imagem)
        {
            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            if (utilizador == null) return Unauthorized();

            if (imagem == null || imagem.Length == 0)
            {
                TempData["Erro"] = "Imagem inválida.";
                return RedirectToAction("Details", new { id = monumentoId });
            }

            // Garante pasta wwwroot/imagens
            var imagensFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "imagens");
            if (!Directory.Exists(imagensFolder)) Directory.CreateDirectory(imagensFolder);

            // Nome único para evitar conflitos
            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(imagem.FileName);
            var fullPath = Path.Combine(imagensFolder, uniqueName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            // Se não houver principal, define esta como principal
            var isPrincipal = !_context.Imagem.Any(i => i.MonumentoId == monumentoId && i.IsPrincipal);

            var imagemDb = new Imagem
            {
                NomeImagem = uniqueName,
                MonumentoId = monumentoId,
                UtilizadorId = utilizador.Id,
                IsPrincipal = isPrincipal
            };

            _context.Imagem.Add(imagemDb);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = "Imagem enviada com sucesso.";
            return RedirectToAction("Details", new { id = monumentoId });
        }

        /// POST: Remover imagem
        [HttpPost]
        public async Task<IActionResult> DeleteImagem(int id)
        {
            var imagem = await _context.Imagem.FindAsync(id);
            if (imagem == null) return NotFound();

            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && imagem.UtilizadorId != utilizador?.Id)
                return Forbid();

            // Remove ficheiro físico
            var imagensFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "imagens");
            var filePath = Path.Combine(imagensFolder, imagem.NomeImagem);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);

            _context.Imagem.Remove(imagem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = imagem.MonumentoId });
        }

        // ======================
        // VISITAS
        // ======================

        /// POST: Alterna a visita (adiciona ou remove)
        [HttpPost]
        public async Task<IActionResult> ToggleVisita(int id)
        {
            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita == null)
            {
                _context.VisitaMonumento.Add(new VisitaMonumento { MonumentoId = id, UtilizadorId = utilizador.Id, NumeroVisitas = 1 });
            }
            else
            {
                _context.VisitaMonumento.Remove(visita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        /// Incrementa visitas
        [HttpPost]
        public async Task<IActionResult> IncrementarVisita(int id)
        {
            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita == null)
            {
                visita = new VisitaMonumento { MonumentoId = id, UtilizadorId = utilizador.Id, NumeroVisitas = 1 };
                _context.VisitaMonumento.Add(visita);
            }

            visita.NumeroVisitas = Math.Max(1, visita.NumeroVisitas);
            visita.NumeroVisitas++;
            _context.Update(visita);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }

        /// Decrementa visitas
        [HttpPost]
        public async Task<IActionResult> DecrementarVisita(int id)
        {
            var utilizador = await GetOrCreateUtilizadorForCurrentIdentityAsync();
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento
                .FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita == null) return RedirectToAction("Details", new { id });

            visita.NumeroVisitas = visita.NumeroVisitas <= 1 ? 1 : visita.NumeroVisitas - 1;
            _context.Update(visita);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id });
        }
    }
}
