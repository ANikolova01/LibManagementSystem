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
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
             return View(await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard).ToListAsync());
            //var listCheckoutModels = new List<CheckoutFullModel>();

            //var checkouts = await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard).ToListAsync();
            //var patrons = await _context.Patrons.Include(p => p.LibraryCard).ToListAsync();

            //foreach(var checkout in checkouts)
            //{

            //    var checkoutModel = new CheckoutFullModel
            //    {
            //        Id = checkout.Id,
            //        Book = checkout.Book,
            //        LibraryCard = checkout.LibraryCard,
            //        CheckedOutSince = checkout.CheckedOutSince,
            //        CheckedOutUntil = checkout.CheckedOutUntil,
            //        Patron = 
            //    }
            //}
        }

        // GET: Checkouts/Details/5
        [Authorize(Roles = "Admin")]
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
            return now.AddDays(14);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CheckoutFullModel checkoutFullModel)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.FirstName == checkoutFullModel.Patron.FirstName && p.LastName == checkoutFullModel.Patron.LastName && p.Email == checkoutFullModel.Patron.Email);
            var book = await _context.Books.Include(b => b.AvailabilityStatus).Include(b => b.Location).FirstOrDefaultAsync(b => b.Title == checkoutFullModel.Book.Title);
            var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);
            var reservation = await _context.Reservation.Include(l => l.Book).Include(l => l.LibraryCard).FirstOrDefaultAsync(l => l.Book.Id == book.Id && l.LibraryCard.Id == libraryCard.Id);

            if (patron == null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This email is not registered as a user and patron!");
                return View(checkoutFullModel);
            };

            if (patron.AccountStatus == "Pending" || patron.AccountStatus == "Deactivated")
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This patron does not have an approved account!");
                return View(checkoutFullModel);
            };

            foreach (var checkoutItem in libraryCard.Checkouts)
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
                //    CheckoutHistory checkoutHistory = new CheckoutHistory();

                    checkout.Book = book;
                    checkout.LibraryCard = patron.LibraryCard;
                    checkout.CheckedOutSince = checkoutFullModel.CheckedOutSince;
                    checkout.CheckedOutUntil = GetDefaultCheckoutTime(checkout.CheckedOutSince);

                    //checkoutHistory.Book = book;
                    //checkoutHistory.LibraryCard = patron.LibraryCard;
                    //checkoutHistory.CheckedOut = checkoutFullModel.CheckedOutSince;

                    book.CopiesAvailable -= 1;
                    libraryCard.CurrentFees += (book.Cost / 5);

                    if(reservation != null) _context.Remove(reservation);

                    _context.Add(checkout);
                //    _context.Add(checkoutHistory);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Checkout(CheckoutFullModel checkoutFullModel)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch).FirstOrDefaultAsync(p => p.Email == checkoutFullModel.Patron.Email);
            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == checkoutFullModel.Book.Id);
            var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);
            var reservation = await _context.Reservation.Include(l => l.Book).Include(l => l.LibraryCard).FirstOrDefaultAsync(l => l.Book.Id == book.Id && l.LibraryCard.Id == libraryCard.Id);

            if(patron == null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This email is not registered as a user and patron!");

                var checkoutModel = new CheckoutFullModel
                {
                    Book = book,
                    LibraryCard = libraryCard,
                    Patron = patron,
                };

                return View("~/Views/Books/Checkout.cshtml", checkoutModel);
            };

            if(patron.AccountStatus == "Pending" || patron.AccountStatus == "Deactivated")
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This patron does not have an approved account!");

                var checkoutModel = new CheckoutFullModel
                {
                    Book = book,
                    LibraryCard = libraryCard,
                    Patron = patron,
                };

                return View("~/Views/Books/Checkout.cshtml", checkoutModel);
            };

            foreach (var checkoutItem in libraryCard.Checkouts)
            {
                if (checkoutItem.Book == book)
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item has already been checked out on this library card!");

                    var checkoutModel = new CheckoutFullModel
                    {
                        Book = book,
                        LibraryCard = libraryCard,
                        Patron = patron,
                    };

                    return View("~/Views/Books/Checkout.cshtml", checkoutModel);
                }
            }

            if (book.CopiesAvailable > 0)
            {
                Checkout checkout = new Checkout();

                checkout.Book = book;
                checkout.LibraryCard = patron.LibraryCard;
                checkout.CheckedOutSince = DateTime.Now;
                checkout.CheckedOutUntil = GetDefaultCheckoutTime(DateTime.Now);

                book.CopiesAvailable -= 1;
                libraryCard.CurrentFees += (book.Cost / 5);

                if (reservation != null) _context.Remove(reservation);

                _context.Add(checkout);
                _context.Update(book);
                _context.Update(libraryCard);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
                
            }
            else
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "No copies of this book are currently available for loaning!");
                return View(checkoutFullModel);
            }

        }

        [HttpPost, ActionName("CheckInBook")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckInBook(CheckoutFullModel checkoutFullModel)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch).FirstOrDefaultAsync(p => p.Email == checkoutFullModel.Patron.Email);
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == checkoutFullModel.Book.Id);
            LibraryCard libraryCard = patron.LibraryCard;
        //    var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);

            var checkout = await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard).FirstOrDefaultAsync(m => m.LibraryCard.Id == libraryCard.Id && m.Book.Id == book.Id);

            if(checkout == null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item has not been checked out on this library card!");

                var checkoutModel = new CheckoutFullModel
                {
                    Book = book,
                    LibraryCard = libraryCard,
                    Patron = patron,
                };

                return View("~/Views/Books/Checkin.cshtml", checkoutModel);
            }

            var checkoutHistory = new CheckoutHistory
            {
                Book = book,
                LibraryCard = libraryCard,
                CheckedOut = checkout.CheckedOutSince,
                CheckedIn = DateTime.Now,
            };

            book.CopiesAvailable += 1;
            libraryCard.CurrentFees -= (book.Cost / 5);


            _context.Remove(checkout);
            _context.CheckoutHistories.Add(checkoutHistory);
            _context.Update(book);
            _context.Update(libraryCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("CheckIn")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckIn(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard).FirstOrDefaultAsync(m => m.Id == id);

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == checkout.Book.Title);
            var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == checkout.LibraryCard.Id);

            var checkoutHistory = new CheckoutHistory
            {
                Book = book,
                LibraryCard = libraryCard,
                CheckedOut = checkout.CheckedOutSince,
                CheckedIn = DateTime.Now,
            };

            book.CopiesAvailable += 1;
            libraryCard.CurrentFees -= (book.Cost / 5);


            _context.Remove(checkout);
            _context.CheckoutHistories.Add(checkoutHistory);
            _context.Update(book);
            _context.Update(libraryCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Checkouts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
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

        // POST: Checkouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, CheckoutFullModel checkoutModel)
        {
            if (id != checkoutModel.Id)
            {
                return NotFound();
            }

            Checkout checkout = new Checkout
            {
                Id=checkoutModel.Id,
                LibraryCard = checkoutModel.LibraryCard,
                Book = checkoutModel.Book,
                CheckedOutSince = checkoutModel.CheckedOutSince,
                CheckedOutUntil = checkoutModel.CheckedOutUntil,
            };

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
            //}
            //return View(checkout);
        }

        // GET: Checkouts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard)
                .FirstOrDefaultAsync(m => m.Id == id);

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == checkout.LibraryCard.Id);

            if (checkout == null)
            {
                return NotFound();
            }
            var checkoutModel = new CheckoutFullModel
            {
                Id = checkout.Id,
                Patron = patron,
                LibraryCard = checkout.LibraryCard,
                CheckedOutSince = checkout.CheckedOutSince,
                CheckedOutUntil = checkout.CheckedOutUntil,
                Book = checkout.Book,
            };

            return View(checkoutModel);
        }

        // POST: Checkouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkout = await _context.Checkouts.Include(c => c.Book).Include(c => c.LibraryCard).FirstOrDefaultAsync(m => m.Id == id);
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == checkout.Book.Title);
            var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).FirstOrDefaultAsync(l => l.Id == checkout.LibraryCard.Id);

            libraryCard.CurrentFees -= (book.Cost / 5);
            book.CopiesAvailable += 1;

            _context.Checkouts.Remove(checkout);
            _context.Books.Update(book);
            _context.LibraryCards.Update(libraryCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckoutExists(int id)
        {
            return _context.Checkouts.Any(e => e.Id == id);
        }
    }
}
