using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using appMonumentos.Models;

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
            var applicationDbContext = _context.Monumento.Include(m => m.Localidade).Include(m => m.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Monumento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monumento = await _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monumento == null)
            {
                return NotFound();
            }

            return View(monumento);
        }

        // GET: Monumento/Create
        public IActionResult Create()
        {
            ViewData["LocalidadeId"] = new SelectList(_context.Set<Localidade>(), "Id", "Id");
            ViewData["UtilizadorId"] = new SelectList(_context.Set<Utilizador>(), "Id", "Nome");
            return View();
        }

        // POST: Monumento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,UtilizadorId,LocalidadeId")] Monumento monumento)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monumento);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocalidadeId"] = new SelectList(_context.Set<Localidade>(), "Id", "Id", monumento.LocalidadeId);
            ViewData["UtilizadorId"] = new SelectList(_context.Set<Utilizador>(), "Id", "Nome", monumento.UtilizadorId);
            return View(monumento);
        }

        // GET: Monumento/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento == null)
            {
                return NotFound();
            }
            ViewData["LocalidadeId"] = new SelectList(_context.Set<Localidade>(), "Id", "Id", monumento.LocalidadeId);
            ViewData["UtilizadorId"] = new SelectList(_context.Set<Utilizador>(), "Id", "Nome", monumento.UtilizadorId);
            return View(monumento);
        }

        // POST: Monumento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Designacao,Endereco,Coordenadas,EpocaConstrucao,Descricao,UtilizadorId,LocalidadeId")] Monumento monumento)
        {
            if (id != monumento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monumento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonumentoExists(monumento.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocalidadeId"] = new SelectList(_context.Set<Localidade>(), "Id", "Id", monumento.LocalidadeId);
            ViewData["UtilizadorId"] = new SelectList(_context.Set<Utilizador>(), "Id", "Nome", monumento.UtilizadorId);
            return View(monumento);
        }

        // GET: Monumento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var monumento = await _context.Monumento
                .Include(m => m.Localidade)
                .Include(m => m.Utilizador)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (monumento == null)
            {
                return NotFound();
            }

            return View(monumento);
        }

        // POST: Monumento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monumento = await _context.Monumento.FindAsync(id);
            if (monumento != null)
            {
                _context.Monumento.Remove(monumento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MonumentoExists(int id)
        {
            return _context.Monumento.Any(e => e.Id == id);
        }
    }
}
