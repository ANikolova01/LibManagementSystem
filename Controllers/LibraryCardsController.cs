using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;

namespace LibraryManagementSystem.Controllers
{
    public class LibraryCardsController : Controller
    {
        private readonly LibraryDbContext _context;

        public LibraryCardsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: LibraryCards
        public async Task<IActionResult> Index()
        {
            return View(await _context.LibraryCards.ToListAsync());
        }

        // GET: LibraryCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryCard = await _context.LibraryCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryCard == null)
            {
                return NotFound();
            }

            return View(libraryCard);
        }

        // GET: LibraryCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LibraryCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CurrentFees,Issued")] LibraryCard libraryCard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(libraryCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libraryCard);
        }

        // GET: LibraryCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryCard = await _context.LibraryCards.FindAsync(id);
            if (libraryCard == null)
            {
                return NotFound();
            }
            return View(libraryCard);
        }

        // POST: LibraryCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CurrentFees,Issued")] LibraryCard libraryCard)
        {
            if (id != libraryCard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(libraryCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryCardExists(libraryCard.Id))
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
            return View(libraryCard);
        }

        // GET: LibraryCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryCard = await _context.LibraryCards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryCard == null)
            {
                return NotFound();
            }

            return View(libraryCard);
        }

        // POST: LibraryCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libraryCard = await _context.LibraryCards.FindAsync(id);
            _context.LibraryCards.Remove(libraryCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryCardExists(int id)
        {
            return _context.LibraryCards.Any(e => e.Id == id);
        }
    }
}
