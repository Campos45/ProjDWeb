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
        private readonly IWebHostEnvironment _webHostEnvironment; // ambiente para aceder a pastas wwwroot

        // Construtor para injeção de dependências
        public MonumentoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Monumento
        // Lista todos os monumentos, incluindo os dados da localidade e do utilizador que criou
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Monumento/Details/5
        // Mostra os detalhes de um monumento, incluindo localidade, utilizador, imagens com comentários e visitas
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
        // Formulário para criar um novo monumento (só para utilizadores autenticados)
        [Authorize]
        public IActionResult Create()
        {
            // Prepara as listas dropdown para selecionar localidade e utilizador
            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade");
            ViewData["UtilizadorId"] = new SelectList(_context.Utilizador, "Id", "Nome");
            return View();
        }

        // POST: Monumento/Create
        // Recebe os dados do formulário para criar um monumento novo (valida modelo)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,UtilizadorId,LocalidadeId")] Monumento monumento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monumento); // adiciona o monumento
                await _context.SaveChangesAsync(); // guarda na BD
                return RedirectToAction(nameof(Index)); // redireciona para a lista
            }

            // Se o modelo for inválido, recarrega as listas para o formulário e mostra de novo
            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
            ViewData["UtilizadorId"] = new SelectList(_context.Utilizador, "Id", "Nome", monumento.UtilizadorId);
            return View(monumento);
        }

        // GET: Monumento/Edit/5
        // Formulário para editar monumento (só autenticados)
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null) return NotFound();

            // Prepara dropdowns com os valores atuais selecionados
            ViewData["LocalidadeId"] = new SelectList(_context.Localidade, "Id", "NomeLocalidade", monumento.LocalidadeId);
            ViewData["UtilizadorId"] = new SelectList(_context.Utilizador, "Id", "Nome", monumento.UtilizadorId);
            return View(monumento);
        }

        // POST: Monumento/Edit/5
        // Atualiza os dados do monumento no formulário editado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,UtilizadorId,LocalidadeId")] Monumento monumento)
        {
            if (id != monumento.Id) return NotFound();

            if (!ModelState.IsValid) return View(monumento);

            try
            {
                _context.Update(monumento); // atualiza
                await _context.SaveChangesAsync(); // guarda alterações
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonumentoExists(monumento.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Monumento/Delete/5
        // Mostra página para confirmar eliminação (apenas Admin)
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
        // Confirma e executa a eliminação do monumento (apenas Admin)
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

        // Verifica se um monumento existe na BD
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
            var username = User.Identity.Name; // nome do utilizador autenticado
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            if (imagem == null || imagem.Length == 0)
            {
                TempData["Erro"] = "Imagem inválida.";
                return RedirectToAction("Details", new { id = monumentoId });
            }

            var monumento = await _context.Monumento.Include(m => m.Imagens).FirstOrDefaultAsync(m => m.Id == monumentoId);
            if (monumento == null) return NotFound();

            // Gera nome único para imagem para evitar conflitos
            var nomeImagem = Guid.NewGuid().ToString() + Path.GetExtension(imagem.FileName);
            var caminhoPasta = Path.Combine(_webHostEnvironment.WebRootPath, "imagens");

            // Cria pasta caso não exista
            if (!Directory.Exists(caminhoPasta))
                Directory.CreateDirectory(caminhoPasta);

            var caminhoCompleto = Path.Combine(caminhoPasta, nomeImagem);

            // Grava o ficheiro no disco
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            // Define a imagem principal apenas se o utilizador for o criador do monumento e ainda não houver imagem principal
            bool isPrincipal = monumento.UtilizadorId == utilizador.Id && !monumento.Imagens.Any(i => i.IsPrincipal);

            // Cria novo objeto imagem
            var novaImagem = new Imagem
            {
                NomeImagem = nomeImagem,
                MonumentoId = monumentoId,
                UtilizadorId = utilizador.Id,
                IsPrincipal = isPrincipal
            };

            _context.Imagem.Add(novaImagem); // adiciona à BD
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = monumentoId });
        }

        // POST: Monumento/DeleteImagem
        // Permite apagar uma imagem (só o criador da imagem ou admin podem apagar)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> DeleteImagem(int id)
        {
            var imagem = await _context.Imagem.FindAsync(id);
            if (imagem == null) return NotFound();

            var username = User.Identity.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            var isAdmin = User.IsInRole("Admin");

            // Verifica permissão para apagar (admin ou dono da imagem)
            if (!isAdmin && imagem.UtilizadorId != utilizador?.Id)
                return Forbid();

            // Apaga ficheiro físico da imagem
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "imagens", imagem.NomeImagem);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            _context.Imagem.Remove(imagem); // remove da BD
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = imagem.MonumentoId });
        }

        // POST: Monumento/ToggleVisita
        // Marca ou desmarca um monumento como visitado pelo utilizador autenticado
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ToggleVisita(int id)
        {
            var username = User.Identity.Name;
            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.Username == username);
            if (utilizador == null) return Unauthorized();

            // Procura se já existe uma visita registada para o utilizador e monumento
            var visita = await _context.VisitaMonumento.FirstOrDefaultAsync(v => v.MonumentoId == id && v.UtilizadorId == utilizador.Id);

            if (visita != null)
                _context.VisitaMonumento.Remove(visita); // remove visita se existir (toggle off)
            else
                _context.VisitaMonumento.Add(new VisitaMonumento { MonumentoId = id, UtilizadorId = utilizador.Id }); // adiciona visita (toggle on)

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }
    }
}
