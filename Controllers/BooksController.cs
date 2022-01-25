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

        // GET: Books/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
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
                return View("NotFound");
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
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus).FirstOrDefaultAsync(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Book book)
        {

            if (id != book.Id)
            {
                return NotFound();
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

            //       Book updateBook = book;
            Book updateBook = oldBook;
            try
                {
                    _context.Update(updateBook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            //return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(b => b.Location).Include(b => b.AvailabilityStatus)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
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
