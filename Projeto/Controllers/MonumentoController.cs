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
        private readonly ApplicationDbContext _context; // contexto da BD
        private readonly IWebHostEnvironment _webHostEnvironment; // para aceder a wwwroot

        public MonumentoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Monumento
        // Lista todos os monumentos (inclui Localidade e Utilizador para mostrar na view)
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Monumento/Details/5
        // Mostra detalhes (imagens, comentários e visitas também)
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
        // Apenas utilizadores autenticados podem criar
        [Authorize]
        public async Task<IActionResult> Create()
        {
            // Lista de localidade para dropdown
            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade");

            // Obter utilizador autenticado para mostrar o nome na view
            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);

            // ViewBag com nome e id do utilizador autenticado (nome a mostrar)
            ViewBag.CurrentUserName = utilizador?.Nome ?? username ?? "Desconhecido";
            ViewBag.CurrentUserId = utilizador?.Id ?? 0;

            return View();
        }

        // POST: Monumento/Create
        // associa o monumento ao utilizador autenticado (não permite escolher outro utilizador)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,LocalidadeId")] Monumento monumento)
        {
            // obter utilizador autenticado
            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                // garantir que o UtilizadorId é o do utilizador autenticado
                monumento.UtilizadorId = utilizador.Id;

                _context.Add(monumento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // se houver erro, recarregar dropdown de localidade e voltar à view
            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
            ViewBag.CurrentUserName = utilizador.Nome;
            ViewBag.CurrentUserId = utilizador.Id;
            return View(monumento);
        }

        // GET: Monumento/Edit/5
        // Só o criador do monumento ou Admin podem aceder
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            // verifica permissões: dono ou admin
            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && monumento.UtilizadorId != utilizador?.Id)
                return Forbid();

            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);

            // Mostrar nome do dono para informação
            var dono = await _context.Utilizador.FindAsync(monumento.UtilizadorId);
            ViewBag.DonoNome = dono?.Nome ?? dono?.Username ?? "Desconhecido";

            return View(monumento);
        }

        // POST: Monumento/Edit/5
        // Actualiza apenas os campos editáveis e preserva o UtilizadorId original
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,LocalidadeId")] Monumento monumento)
        {
            if (id != monumento.Id) return NotFound();

            // obter o monumento actual da BD (para preservar UtilizadorId e verificar permissões)
            var existing = await _context.Monumento.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (existing == null) return NotFound();

            var username = User.Identity?.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && existing.UtilizadorId != utilizador?.Id)
                return Forbid();

            if (!ModelState.IsValid)
            {
                ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
                var dono = await _context.Utilizador.FindAsync(existing.UtilizadorId);
                ViewBag.DonoNome = dono?.Nome ?? dono?.Username ?? "Desconhecido";
                return View(monumento);
            }

            try
            {
                // preservar o UtilizadorId do existente
                monumento.UtilizadorId = existing.UtilizadorId;

                // actualizar campos via Attach/Entry
                _context.Entry(monumento).State = EntityState.Modified;
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
        // Página de confirmação. Só dono ou Admin podem aceder.
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento
                .Include(m => m.Localidade)
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
        // Confirma deleção (aplica mesma verificação de permissões)
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

        private bool MonumentoExists(int id)
        {
            return _context.Monumento.Any(e => e.Id == id);
        }

        // POST: Monumento/UploadImagem
        // Permite a um utilizador autenticado carregar uma imagem para um monumento
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
        // Só o dono da imagem ou Admin podem apagar
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
        // Marca / desmarca visita do utilizador autenticado
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
