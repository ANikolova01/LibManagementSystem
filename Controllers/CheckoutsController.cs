using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.Enums;
using LibraryManagementSystem.Data;

namespace LibraryManagementSystem.Controllers
{
    public class CheckoutsController : Controller
    {
        private readonly LibraryDbContext _context;

        public CheckoutsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Checkouts
        public async Task<IActionResult> Index()
        {
            return View(await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard).ToListAsync());
        }

        // GET: Checkouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard)
                .FirstOrDefaultAsync(m => m.Id == id);

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == checkout.LibraryCard.Id);

            var checkoutModel = new CheckoutFullModel
            {
                Id = checkout.Id,
                Patron = patron,
                LibraryCard = checkout.LibraryCard,
                CheckedOutSince = checkout.CheckedOutSince,
                CheckedOutUntil = checkout.CheckedOutUntil,
                Book = checkout.Book,
            };

            if (checkout == null)
            {
                return NotFound();
            }

            return View(checkoutModel);
        }

        private DateTime GetDefaultCheckoutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        // GET: Checkouts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Checkouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CheckoutFullModel checkoutFullModel)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.FirstName == checkoutFullModel.Patron.FirstName && p.LastName == checkoutFullModel.Patron.LastName);
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == checkoutFullModel.Book.Title);
            var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);


            foreach(var checkoutItem in libraryCard.Checkouts)
            {
                if(checkoutItem.Book == book)
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item has already been checked out on this library card!");
                    return View(checkoutFullModel);
                }
            }

            if (book.CopiesAvailable > 0)
            {
                if(book.AvailabilityStatus.Name == "LOST")
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "The item is lost and cannot be checked out!");
                    return View(checkoutFullModel);
                }
                else if(book.AvailabilityStatus.Name == "UNKNOWN_CONDITION")
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "The item is in unknown whereabouts and condition and cannot be checked out!");
                    return View(checkoutFullModel);
                }
                else if(book.AvailabilityStatus.Name == "DESTROYED")
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "The item has been destroyed and cannot be checked out!");
                    return View(checkoutFullModel);
                }
                else
                {
                    Checkout checkout = new Checkout();
                    CheckoutHistory checkoutHistory = new CheckoutHistory();

                    checkout.Book = book;
                    checkout.LibraryCard = patron.LibraryCard;
                    checkout.CheckedOutSince = checkoutFullModel.CheckedOutSince;
                    checkout.CheckedOutUntil = GetDefaultCheckoutTime(checkout.CheckedOutSince);

                    checkoutHistory.Book = book;
                    checkoutHistory.LibraryCard = patron.LibraryCard;
                    checkoutHistory.CheckedOut = checkoutFullModel.CheckedOutSince;

                    book.CopiesAvailable -= 1;
                    libraryCard.CurrentFees += book.Cost;

                    _context.Add(checkout);
                    _context.Add(checkoutHistory);
                    _context.Update(book);
                    _context.Update(libraryCard);

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            else
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "No copies of this book are currently available for loaning!");
                return View(checkoutFullModel);
            }

        }

        public async Task<IActionResult> Checkout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts.Include(c => c.LibraryCard).FirstOrDefaultAsync(c => c.Id == id);

            checkout.CheckedOutUntil = DateTime.Now;

            _context.Update(checkout);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Checkouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts.FindAsync(id);
            if (checkout == null)
            {
                return NotFound();
            }
            return View(checkout);
        }

        // POST: Checkouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CheckedOutSince,CheckedOutUntil")] Checkout checkout)
        {
            if (id != checkout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckoutExists(checkout.Id))
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
            return View(checkout);
        }

        // GET: Checkouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkout == null)
            {
                return NotFound();
            }

            return View(checkout);
        }

        // POST: Checkouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkout = await _context.Checkouts.FindAsync(id);
            _context.Checkouts.Remove(checkout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckoutExists(int id)
        {
            return _context.Checkouts.Any(e => e.Id == id);
        }
    }
}
