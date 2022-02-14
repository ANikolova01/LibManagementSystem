using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.LibraryCards.ToListAsync());
        }

        // GET: LibraryCards/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid? id)
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
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == id);

            LibraryCardFullModel libraryCardModel = new LibraryCardFullModel()
            {
                LibraryCard = libraryCard,
                Patron = patron,
            };

            return View(libraryCardModel);
        }

        // GET: LibraryCards/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LibraryCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(LibraryCardFullModel libraryCardModel)
        {
            var patron = await _context.Patrons.Include(p => p.HomeLibraryBranch).Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.Email == libraryCardModel.Patron.Email);

            if (patron == null)
            {
               return View(libraryCardModel);
            }
            else if(patron.LibraryCard != null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Patron already has Library Card!");
                return View(libraryCardModel);
            }
            var libraryCard = new LibraryCard()
            {
                Id = new Guid(),
                CurrentFees = 0,
                Issued = libraryCardModel.LibraryCard.Issued
            };

            patron.LibraryCard = libraryCard;
            patron.OverdueFees -= 10;
            patron.AccountStatus = "Approved";
            _context.Add(libraryCard);
            _context.Update(patron);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

          //  return View(libraryCard);
        }

        // GET: LibraryCards/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
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
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == id);

            var libraryCardModel = new LibraryCardFullModel
            {
                LibraryCard = libraryCard,
                Patron = patron
            };
            return View(libraryCardModel);
        }

        // POST: LibraryCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, LibraryCardFullModel libraryCardModel)
        {
            if (id != libraryCardModel.LibraryCard.Id)
            {
                return NotFound();
            }

            LibraryCard libraryCard = new LibraryCard
            {
                Id = libraryCardModel.LibraryCard.Id,
                Issued = libraryCardModel.LibraryCard.Issued,
                CurrentFees = libraryCardModel.LibraryCard.CurrentFees,

            };

            //if (ModelState.IsValid)
            //{
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
            //}
            //return View(libraryCard);
        }

        // GET: LibraryCards/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
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

            var patron = await _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch).FirstOrDefaultAsync(p => p.LibraryCard.Id == id);

            LibraryCardFullModel libraryCardModel = new LibraryCardFullModel
            {
                Patron = patron,
                LibraryCard = libraryCard,
            };

            return View(libraryCardModel);
        }

        // POST: LibraryCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var libraryCard = await _context.LibraryCards.FindAsync(id);
            _context.LibraryCards.Remove(libraryCard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryCardExists(Guid id)
        {
            return _context.LibraryCards.Any(e => e.Id == id);
        }
    }
}
