using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class CheckoutHistoriesController : Controller
    {
        private readonly LibraryDbContext _context;

        public CheckoutHistoriesController(LibraryDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: CheckoutHistories
        public async Task<IActionResult> Index()
        {
            return View(await _context.CheckoutHistories.Include(c => c.Book).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: CheckoutHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("~/Views/CheckoutHistories/NotFound.cshtml");
            }

            var checkoutHistory = await _context.CheckoutHistories.Include(c => c.Book).Include(c => c.LibraryCard)
                .FirstOrDefaultAsync(m => m.Id == id);
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == checkoutHistory.LibraryCard.Id);

            if (checkoutHistory == null)
            {
                return View("~/Views/CheckoutHistories/NotFound.cshtml");
            }

            var checkoutHistoryModel = new CheckoutHistoryFullModel
            {
                Patron = patron,
                LibraryCard = checkoutHistory.LibraryCard,
                Book = checkoutHistory.Book,
                CheckedIn = checkoutHistory.CheckedIn,
                CheckedOut = checkoutHistory.CheckedOut,
            };

            return View(checkoutHistoryModel);
        }

        [Authorize(Roles = "Admin")]
        // GET: CheckoutHistories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CheckoutHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CheckoutHistoryFullModel checkoutHistoryModel)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.FirstName == checkoutHistoryModel.Patron.FirstName
            && p.LastName == checkoutHistoryModel.Patron.LastName && p.Email == checkoutHistoryModel.Patron.Email);

            var book = await _context.Books.Include(b => b.Location).FirstOrDefaultAsync(b => b.Title == checkoutHistoryModel.Book.Title);

            CheckoutHistory checkoutHistory = new CheckoutHistory
            {
                Book = book,
                LibraryCard = patron.LibraryCard,
                CheckedOut = checkoutHistoryModel.CheckedOut,
                CheckedIn = checkoutHistoryModel.CheckedIn,
            };

            _context.Add(checkoutHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: CheckoutHistories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("~/Views/CheckoutHistories/NotFound.cshtml");
            }

            var checkoutHistory = await _context.CheckoutHistories.Include(c => c.Book).ThenInclude(b => b.Location).Include(c => c.LibraryCard).FirstOrDefaultAsync(c => c.Id == id);
            if (checkoutHistory == null)
            {
                return View("~/Views/CheckoutHistories/NotFound.cshtml");
            }

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == checkoutHistory.LibraryCard.Id);

            var checkoutHistoryModel = new CheckoutHistoryFullModel
            {
                Id = checkoutHistory.Id,
                LibraryCard = checkoutHistory.LibraryCard,
                Book = checkoutHistory.Book,
                Patron = patron,
                CheckedIn = checkoutHistory.CheckedIn,
                CheckedOut = checkoutHistory.CheckedOut
            };
            return View(checkoutHistoryModel);
        }

        // POST: CheckoutHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CheckoutHistoryFullModel checkoutHistoryModel)
        {
            if (id != checkoutHistoryModel.Id)
            {
                return View("~/Views/CheckoutHistories/NotFound.cshtml");
            }

            var checkoutHistory = new CheckoutHistory
            {
                Id = checkoutHistoryModel.Id,
                Book = checkoutHistoryModel.Book,
                LibraryCard = checkoutHistoryModel.LibraryCard,
                CheckedOut = checkoutHistoryModel.CheckedOut,
                CheckedIn= checkoutHistoryModel.CheckedIn,
            };

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(checkoutHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckoutHistoryExists(checkoutHistory.Id))
                    {
                        return View("~/Views/CheckoutHistories/NotFound.cshtml");
                    }
                else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            //return View(checkoutHistory);
        }

        // GET: CheckoutHistories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("~/Views/CheckoutHistories/NotFound.cshtml");
            }

            var checkoutHistory = await _context.CheckoutHistories.Include(c => c.Book).ThenInclude(b => b.Location).Include(c => c.LibraryCard).FirstOrDefaultAsync(c => c.Id == id);

            if (checkoutHistory == null)
            {
                return View("~/Views/CheckoutHistories/NotFound.cshtml");
            }

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == checkoutHistory.LibraryCard.Id);
            var checkoutHistoryModel = new CheckoutHistoryFullModel
            {
                Id = checkoutHistory.Id,
                LibraryCard = checkoutHistory.LibraryCard,
                Book = checkoutHistory.Book,
                Patron = patron,
                CheckedIn = checkoutHistory.CheckedIn,
                CheckedOut = checkoutHistory.CheckedOut
            };

            return View(checkoutHistoryModel);
        }

        // POST: CheckoutHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
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
