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
    public class PatronsController : Controller
    {
        private readonly LibraryDbContext _context;

        public PatronsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Patrons
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch).ToListAsync());
        }

        // GET: Patrons/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patron = await _context.Patrons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patron == null)
            {
                return NotFound();
            }

            return View(patron);
        }

        // GET: Patrons/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patrons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatedOn,UpdatedOn,FirstName,LastName,Address,DateOfBirth,Email,Telephone,OverdueFees,AccountStatus")] Patron patron)
        {
            if (ModelState.IsValid)
            {
                patron.Id = new Guid();
                _context.Add(patron);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(patron);
        }

        // GET: Patrons/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patron = _context.Patrons.Include(p => p.HomeLibraryBranch).Include(p => p.LibraryCard).FirstOrDefault(p => p.Id == id);
            if (patron == null)
            {
                return NotFound();
            }
            return View(patron);
        }

        // POST: Patrons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Patron patron)
        {
            
            if (id != patron.Id)
            {
                return NotFound();
            }
            var patronFound = await _context.Patrons.AsNoTracking().Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.Id == id);
     //       var branch = patronFound.HomeLibraryBranch;

            if(patron.AccountStatus == "Approved" && patronFound.LibraryCard == null)
            {
                patron.OverdueFees -= 10;

                LibraryCard newCard = new LibraryCard();
                newCard.Id = new Guid();
                newCard.Issued = DateTime.Now;
                newCard.CurrentFees = 0;
                _context.LibraryCards.Add(newCard);
                var cardModel = newCard;
                patron.LibraryCard = cardModel;

            }

            else if(patron.AccountStatus == "Approved" && patronFound.LibraryCard != null)
            {
                patron.OverdueFees -= 10;
                patron.AccountStatus = "Approved";
            }

            else if (patron.AccountStatus == "Deactivated" && patronFound.LibraryCard != null)
            {
                patron.OverdueFees += 10;
                patron.AccountStatus = "Deactivated";

            }

            Patron patronModel = patron;

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(patronModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatronExists(patron.Id))
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
            //return View(patron);
        }

        // GET: Patrons/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patron = await _context.Patrons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (patron == null)
            {
                return NotFound();
            }

            return View(patron);
        }

        // POST: Patrons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var patron = await _context.Patrons.FindAsync(id);
            _context.Patrons.Remove(patron);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatronExists(Guid id)
        {
            return _context.Patrons.Any(e => e.Id == id);
        }
    }
}
