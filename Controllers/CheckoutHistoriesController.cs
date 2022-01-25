using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class CheckoutHistoriesController : Controller
    {
        private readonly LibraryDbContext _context;

        public CheckoutHistoriesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: CheckoutHistories
        public async Task<IActionResult> Index()
        {
            return View(await _context.CheckoutHistories.ToListAsync());
        }

        // GET: CheckoutHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutHistory = await _context.CheckoutHistories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkoutHistory == null)
            {
                return NotFound();
            }

            return View(checkoutHistory);
        }

        // GET: CheckoutHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CheckoutHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CheckedOut,CheckedIn")] CheckoutHistory checkoutHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(checkoutHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(checkoutHistory);
        }

        // GET: CheckoutHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutHistory = await _context.CheckoutHistories.FindAsync(id);
            if (checkoutHistory == null)
            {
                return NotFound();
            }
            return View(checkoutHistory);
        }

        // POST: CheckoutHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CheckedOut,CheckedIn")] CheckoutHistory checkoutHistory)
        {
            if (id != checkoutHistory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkoutHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckoutHistoryExists(checkoutHistory.Id))
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
            return View(checkoutHistory);
        }

        // GET: CheckoutHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkoutHistory = await _context.CheckoutHistories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkoutHistory == null)
            {
                return NotFound();
            }

            return View(checkoutHistory);
        }

        // POST: CheckoutHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkoutHistory = await _context.CheckoutHistories.FindAsync(id);
            _context.CheckoutHistories.Remove(checkoutHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckoutHistoryExists(int id)
        {
            return _context.CheckoutHistories.Any(e => e.Id == id);
        }
    }
}
