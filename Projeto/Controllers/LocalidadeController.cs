using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // Construtor que recebe o contexto da base de dados por injeção de dependência
        public LocalidadeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Localidade
        // Mostra a lista de todas as localidades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Localidade.ToListAsync());
        }

        // GET: Localidade/Details/5
        // Mostra os detalhes de uma localidade específica
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // Se o id for nulo, devolve erro 404
            }

            var localidade = await _context.Localidade
                .FirstOrDefaultAsync(m => m.Id == id); // Procura a localidade com o ID dado
            if (localidade == null)
            {
                return NotFound(); // Se não encontrar, devolve erro 404
            }

            return View(localidade); // Mostra a localidade encontrada
        }

        // GET: Localidade/Create
        // Mostra o formulário de criação de nova localidade
        public IActionResult Create()
        {
            return View();
        }

        // POST: Localidade/Create
        // Processa o envio do formulário de criação de localidade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomeLocalidade")] Localidade localidade)
        {
            if (ModelState.IsValid) // Verifica se o modelo está válido
            {
                _context.Add(localidade); // Adiciona a nova localidade à base de dados
                await _context.SaveChangesAsync(); // Guarda as alterações
                return RedirectToAction(nameof(Index)); // Redireciona para a lista
            }
            return View(localidade); // Se houver erro, volta ao formulário
        }

        // GET: Localidade/Edit/5
        // Mostra o formulário de edição da localidade com o ID indicado
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var localidade = await _context.Localidade.FindAsync(id); // Procura a localidade
            if (localidade == null)
            {
                return NotFound();
            }
            return View(localidade);
        }

        // POST: Localidade/Edit/5
        // Processa o envio do formulário de edição
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeLocalidade")] Localidade localidade)
        {
            if (id != localidade.Id) // Verifica se o ID na URL corresponde ao da localidade
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(localidade); // Atualiza a localidade
                    await _context.SaveChangesAsync(); // Guarda as alterações
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocalidadeExists(localidade.Id)) // Verifica se a localidade ainda existe
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Lança o erro se for outro problema
                    }
                }
                return RedirectToAction(nameof(Index)); // Redireciona para a lista
            }
            return View(localidade); // Se houver erro, volta ao formulário
        }

        // GET: Localidade/Delete/5
        // Mostra a confirmação para apagar uma localidade
        [Authorize(Roles = "Admin")] // Só administradores podem aceder
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var localidade = await _context.Localidade
                .FirstOrDefaultAsync(m => m.Id == id);
            if (localidade == null)
            {
                return NotFound();
            }

            return View(localidade);
        }

        // POST: Localidade/Delete/5
        // Processa a confirmação de eliminação
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // Só administradores podem apagar
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var localidade = await _context.Localidade.FindAsync(id);
            if (localidade != null)
            {
                _context.Localidade.Remove(localidade); // Remove a localidade
            }

            await _context.SaveChangesAsync(); // Guarda as alterações
            return RedirectToAction(nameof(Index));
        }

        // Verifica se uma localidade existe na base de dados
        private bool LocalidadeExists(int id)
        {
            return _context.Localidade.Any(e => e.Id == id);
        }
    }
}
