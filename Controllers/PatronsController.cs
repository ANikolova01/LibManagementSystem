using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using LibraryManagementSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class PatronsController : Controller
    {
        private readonly LibraryDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatronsController(LibraryDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Patrons
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch).ToListAsync());
        }

        // GET: Patrons/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return View("~/Views/Patrons/NotFound.cshtml");
            }

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(m => m.Id == id);
            if (patron.LibraryCard != null)
            {
                var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);
                
                var checkoutHistory = await _context.CheckoutHistories.Include(c => c.Book).Include(c => c.LibraryCard).Where(c => c.LibraryCard.Id == patron.LibraryCard.Id).ToListAsync();
                var reservations = await _context.Reservation.Include(c => c.Book).Include(c => c.LibraryCard).Where(r => r.LibraryCard.Id == patron.LibraryCard.Id).ToListAsync();
                if (patron == null)
                {
                    return View("~/Views/Patrons/NotFound.cshtml");
                }

                PatronFullModel patronModel = new PatronFullModel
                {
                    Patron = patron,
                    CheckoutHistory = checkoutHistory,
                    Reservations = reservations
                };

                return View(patronModel);

            }
            else
            {
                PatronFullModel patronModel = new PatronFullModel
                {
                    Patron = patron,
                    CheckoutHistory = null,
                    Reservations = null
                };

                return View(patronModel);
            }

        }

        // GET: Patrons/Details/5
        [Authorize(Roles = "Basic")]
        public async Task<IActionResult> DetailsUser()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return View("~/Views/Patrons/NotFoundUser.cshtml");
            }
            var patron = await _context.Patrons.Include(p => p.LibraryCard).ThenInclude(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(p => p.Email == user.Email);
            var checkoutHistory = await _context.CheckoutHistories.Include(c => c.Book).Include(c => c.LibraryCard).Where(c => c.LibraryCard.Id == patron.LibraryCard.Id).ToListAsync();
            var reservations = await _context.Reservation.Include(c => c.Book).Include(c => c.LibraryCard).Where(r => r.LibraryCard.Id == patron.LibraryCard.Id).ToListAsync();

            if (patron == null)
            {
                return View("~/Views/Patrons/NotFound.cshtml");
            }

            PatronFullModel patronModel = new PatronFullModel
            {
                Patron = patron,
                CheckoutHistory = checkoutHistory,
                Reservations = reservations
            };

            return View(patronModel);
        }

        // GET: Patrons/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patrons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,CreatedOn,UpdatedOn,FirstName,LastName,Address,DateOfBirth,Email,Telephone,OverdueFees,AccountStatus")] Patron patron)
        {
            //patron.LibraryCard = new LibraryCard
            //{
            //    Id = Guid.Empty,
               
            //};
            //if (ModelState.IsValid)
            //{
                patron.Id = new Guid();

                var patronCheck = await _context.Patrons.FirstOrDefaultAsync(p => p.Email == patron.Email);

                if(patronCheck != null)
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Patron already exists!");
                    return View(patron);
                }

                var libraryBranch = await _context.LibraryBranches.FirstOrDefaultAsync(b => b.Name == patron.HomeLibraryBranch.Name);
                if(libraryBranch != null)
                {
                    patron.HomeLibraryBranch = libraryBranch;
                }

                _context.Add(patron);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            //return View(patron);
        }

        // GET: Patrons/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return View("~/Views/Patrons/NotFound.cshtml");
            }

            var patron = await _context.Patrons.Include(p => p.HomeLibraryBranch).Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.Id == id);
            if (patron == null)
            {
                return View("~/Views/Patrons/NotFound.cshtml");
            }
            return View(patron);
        }

        // POST: Patrons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, Patron patron)
        {
            
            if (id != patron.Id)
            {
                return View("~/Views/Patrons/NotFound.cshtml");
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
            else if(patronFound.AccountStatus != "Pending" && patron.AccountStatus == "Pending")
            {
                patron.OverdueFees += 10;
                patron.AccountStatus = "Pending";
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
                      return View("~/Views/Patrons/NotFound.cshtml");
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return View("~/Views/Patrons/NotFound.cshtml");
            }

            var patron = await _context.Patrons.Include(p => p.LibraryCard).ThenInclude(l => l.Checkouts)
                .FirstOrDefaultAsync(m => m.Id == id);

            var userCheck = await _userManager.FindByEmailAsync(patron.Email);
            if (patron == null)
            {
                return View("~/Views/Patrons/NotFound.cshtml");
            }
            if (userCheck != null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "The patron's user account still exists. Please contact administrator to delete the account before deleting the patron's account!");
                return View(patron);
            }

            return View(patron);
        }

        // POST: Patrons/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.Id == id);

            _context.Patrons.Remove(patron);

            if(patron.LibraryCard != null)
            {
                var libraryCard = await _context.LibraryCards.FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);
                var checkouts = await _context.Checkouts.Where(c => c.LibraryCard.Id == libraryCard.Id).ToListAsync();
                var checkoutHistory = await _context.CheckoutHistories.Where(c => c.LibraryCard.Id == libraryCard.Id).ToListAsync();
                var reservations = await _context.Reservation.Where(c => c.LibraryCard.Id == libraryCard.Id).ToListAsync();

                _context.LibraryCards.Remove(libraryCard);
                _context.Checkouts.RemoveRange(checkouts);
                _context.CheckoutHistories.RemoveRange(checkoutHistory);
                _context.Reservation.RemoveRange(reservations);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatronExists(Guid id)
        {
            return _context.Patrons.Any(e => e.Id == id);
        }
    }
}
