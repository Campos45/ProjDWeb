using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    public class MonumentoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MonumentoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Monumento
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Monumento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador)
                .Include(m => m.Imagens)
                    .ThenInclude(i => i.Comentarios)
                    .ThenInclude(c => c.Utilizador)
                .Include(m => m.VisitasMonumento)
                    .ThenInclude(v => v.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null) return NotFound();

            return View(monumento);
        }

        // GET: Monumento/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade");
            ViewData["UtilizadorId"] = new SelectList(_context.Utilizador, "Id", "Nome");
            return View();
        }

        // POST: Monumento/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,UtilizadorId,LocalidadeId")] Monumento monumento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monumento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
            ViewData["UtilizadorId"] = new SelectList(_context.Utilizador, "Id", "Nome", monumento.UtilizadorId);
            return View(monumento);
        }

        // GET: Monumento/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
            ViewData["UtilizadorId"] = new SelectList(_context.Utilizador, "Id", "Nome", monumento.UtilizadorId);
            return View(monumento);
        }

        // POST: Monumento/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,UtilizadorId,LocalidadeId")] Monumento monumento)
        {
            if (id != monumento.Id) return NotFound();

            if (!ModelState.IsValid) return View(monumento);

            try
            {
                _context.Update(monumento);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonumentoExists(monumento.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Monumento/Delete/5
        
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (monumento == null) return NotFound();

            return View(monumento);
        }

        // POST: Monumento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento != null)
                _context.Monumento.Remove(monumento);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonumentoExists(int id)
        {
            return _context.Monumento.Any(e => e.Id == id);
        }

        // POST: Monumento/UploadImagem
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadImagem(int monumentoId, IFormFile imagem)
        {
            var username = User.Identity.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            if (imagem == null || imagem.Length == 0)
            {
                TempData["Erro"] = "Imagem inválida.";
                return RedirectToAction("Details", new { id = monumentoId });
            }

            var monumento = await _context.Monumento.Include(m => m.Imagens).FirstOrDefaultAsync(m => m.Id == monumentoId);
            if (monumento == null) return NotFound();

            var nomeImagem = Guid.NewGuid().ToString() + Path.GetExtension(imagem.FileName);
            var caminhoPasta = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");

            if (!Directory.Exists(caminhoPasta))
                Directory.CreateDirectory(caminhoPasta);

            var caminhoCompleto = Path.Combine(caminhoPasta, nomeImagem);
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            bool isPrincipal = monumento.UtilizadorId == utilizador.Id && !monumento.Imagens.Any(i => i.IsPrincipal);

            var novaImagem = new Imagem
            {
                NomeImagem = nomeImagem,
                MonumentoId = monumentoId,
                UtilizadorId = utilizador.Id,
                IsPrincipal = isPrincipal
            };

            _context.Imagem.Add(novaImagem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = monumentoId });
        }

        // POST: Monumento/DeleteImagem
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteImagem(int id)
        {
            var imagem = await _context.Imagem.FindAsync(id);
            if (imagem == null) return NotFound();

            var username = User.Identity.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && imagem.UtilizadorId != utilizador?.Id)
                return Forbid();

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imagens", imagem.NomeImagem);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.Imagem.Remove(imagem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = imagem.MonumentoId });
        }

        // POST: Monumento/ToggleVisita
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleVisita(int id)
        {
            var username = User.Identity.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            var visita = await _context.VisitaMonumento.FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita != null)
                _context.VisitaMonumento.Remove(visita);
            else
                _context.VisitaMonumento.Add(new VisitaMonumento { MonumentoId = id, UtilizadorId = utilizador.Id });

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }
    }
}
