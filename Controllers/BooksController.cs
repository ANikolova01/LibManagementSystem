using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using LibraryManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly ILibraryBranchService _libraryService;

        public BooksController(LibraryDbContext context, ILibraryBranchService libraryService)
        {
            _context = context;
            _libraryService = libraryService;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            return View(await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).ToListAsync());
        }

        // GET: Books
        public async Task<IActionResult> Index1()
        {
            var books = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).ToListAsync();

            return View(books.Where(b => b.Location.Name == "Downtown"));
        }

        // GET: Books
        public async Task<IActionResult> Index2()
        {
            var books = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).ToListAsync();

            return View(books.Where(b => b.Location.Name == "Oakville"));
        }

        // GET: Books
        public async Task<IActionResult> Index3()
        {
            var books = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).ToListAsync();

            return View(books.Where(b => b.Location.Name == "Pacific Branch"));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Checkout(Guid? id)
        {
            var checkouts = await _context.Checkouts.Include(c => c.Book).Where(c => c.Book.Id == id).ToListAsync();

            var reservations = await _context.Reservation.Include(r => r.Book).Where(r => r.Book.Id == id).ToListAsync();
            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == id);
            var patron = new Patron
            {
                Email = ""
            };
            var newCheckoutModel = new CheckoutFullModel
            {
                Book = book,
                LibraryCard = null,
                Patron = patron,
                
            };

            return View(newCheckoutModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckIn(Guid? id)
        {
            //var checkouts = await _context.Checkouts.Include(c => c.Book).Where(c => c.Book.Id == id).ToListAsync();

            //var reservations = await _context.Reservation.Include(r => r.Book).Where(r => r.Book.Id == id).ToListAsync();
            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == id);
            var patron = new Patron
            {
                Email = ""
            };
            var newCheckoutModel = new CheckoutFullModel
            {
                Book = book,
                LibraryCard = null,
                Patron = patron,

            };

            return View(newCheckoutModel);
        }

        [Authorize(Roles = "Basic, Admin")]
        public async Task<IActionResult> Reservation(Guid? id)
        {
            var checkouts = await _context.Checkouts.Include(c => c.Book).Where(c => c.Book.Id == id).ToListAsync();

            var reservations = await _context.Reservation.Include(r => r.Book).Where(r => r.Book.Id == id).ToListAsync();
            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == id);
            var patron = new Patron
            {
                Email = ""
            };
            var newReservationModel = new ReservationFullModel
            {
                Book = book,
                LibraryCard = null,
                Patron = patron,

            };

            return View(newReservationModel);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return View("~/Views/Books/NotFound.cshtml");
            }

            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return View("~/Views/Books/NotFound.cshtml");
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    book.BookImage = dataStream.ToArray();
                }
            }

            book.Id = Guid.NewGuid();

            LibraryBranch branch = await _context.LibraryBranches.FirstOrDefaultAsync(b => b.Name == book.Location.Name);
            AvailabilityStatus status = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == book.AvailabilityStatus.Name);

            if(branch == null)
            {
                ViewBag.ErrorMessage = $"Branch Name = {book.Location.Name} cannot be found";
                return View("~/Views/Books/NotFound.cshtml");
            }

            book.Location = branch;
            book.AvailabilityStatus = status;

            Book bookModel = book;

            var assetsValue = book.NumberOfCopies * book.Cost;

            branch.TotalAssetValue = branch.TotalAssetValue + assetsValue;
            branch.NumberOfAssets = branch.NumberOfAssets + 1;


                _context.Add(bookModel);

                _context.Update(branch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
 //           }
//            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return View("~/Views/Books/NotFound.cshtml");
            }

            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return View("~/Views/Books/NotFound.cshtml");
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Book book)
        {

            if (id != book.Id)
            {
                return View("~/Views/Books/NotFound.cshtml");
            }
            var oldBook = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == id);

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    book.BookImage = dataStream.ToArray();
                    oldBook.BookImage = book.BookImage;
                }
            }


            if(oldBook.Location.Name != book.Location.Name)
            {
                LibraryBranch branch = await _context.LibraryBranches.FirstOrDefaultAsync(b => b.Name == book.Location.Name);

                oldBook.Location = branch;
            }
            //else
            //{
            //    book.Location = oldBook.Location;
            //}

            if(oldBook.AvailabilityStatus.Name != book.AvailabilityStatus.Name)
            {
                AvailabilityStatus status = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == book.AvailabilityStatus.Name);

                oldBook.AvailabilityStatus = status;
            }


            oldBook.Title = book.Title;
            oldBook.Author = book.Author;
            oldBook.ISBN = book.ISBN;
            oldBook.PublicationYear = book.PublicationYear;
            oldBook.Edition = book.Edition;
            oldBook.Publisher = book.Publisher;
            oldBook.DeweyIndex = book.DeweyIndex;
            oldBook.Language = book.Language;
            oldBook.NoOfPages_LengthTime = book.NoOfPages_LengthTime;
            oldBook.Genre = book.Genre;
            oldBook.Summary = book.Summary;
            oldBook.Cost = book.Cost;
            oldBook.NumberOfCopies = book.NumberOfCopies;
            oldBook.CopiesAvailable = book.CopiesAvailable;
            oldBook.Type = book.Type;

            try
                {
                    _context.Update(oldBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                if (!BookExists(book.Id))
                    {
                        return View("~/Views/Books/NotFound.cshtml");
                    }
                else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            //}
            //return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("~/Views/Books/NotFound.cshtml");
            }

            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return View("~/Views/Books/NotFound.cshtml");
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == id);

            LibraryBranch branch = await _context.LibraryBranches.FirstOrDefaultAsync(b => b.Name == book.Location.Name);

            var assetValue = book.NumberOfCopies * book.Cost;
            branch.TotalAssetValue = branch.TotalAssetValue - assetValue;
            branch.NumberOfAssets = branch.NumberOfAssets - 1;

            Book bookModel = book;

            _context.Books.Remove(bookModel);

            _context.Update(branch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(Guid id)
        {
            return _context.Books.Any(e => e.Id == id);
        }


    }
}
