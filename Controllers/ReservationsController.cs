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
    public class ReservationsController : Controller
    {
        private readonly LibraryDbContext _context;

        public ReservationsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reservation.Include(c => c.Book).Include(c => c.LibraryCard).ToListAsync());
        }

        // GET: Reservations/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.Include(c => c.Book).Include(c => c.LibraryCard)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == reservation.LibraryCard.Id);

            var reservationModel = new ReservationFullModel
            {
                Id = reservation.Id,
                Patron = patron,
                LibraryCard = reservation.LibraryCard,
                HoldPlaced = reservation.HoldPlaced,
                UpdatedOn = reservation.UpdatedOn,
                Book = reservation.Book,
                Status = reservation.Status,
            };

            return View(reservationModel);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ReservationFullModel reservationModel)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.FirstName == reservationModel.Patron.FirstName
            && p.LastName == reservationModel.Patron.LastName && p.Email == reservationModel.Patron.Email);
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Title == reservationModel.Book.Title);
            var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);

            if (patron == null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This email is not registered as a user and patron!");
                return View(reservationModel);
            };

            if (patron.AccountStatus == "Pending" || patron.AccountStatus == "Deactivated")
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This patron does not have an approved account!");
                return View(reservationModel);
            };

            var reservations = await _context.Reservation.Include(r => r.Book).Include(r => r.LibraryCard == libraryCard).FirstOrDefaultAsync();

            foreach (var checkoutItem in libraryCard.Checkouts)
            {
                if (checkoutItem.Book == book)
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item has already been checked out on this library card!");

                    var reservationM = new ReservationFullModel
                    {
                        Book = book,
                        LibraryCard = libraryCard,
                        Patron = patron,
                    };

                    return View("~/Views/Reservations/Create.cshtml", reservationM);
                }
            }

            if (reservationModel.LibraryCard.Id == reservations.LibraryCard.Id)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item has already been reserved on this library card!");
                return View(reservationModel);
            }
            else if (book.AvailabilityStatus.Name == "LOST")
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "The item is lost and cannot be reserved!");
                return View(reservationModel);
            }
            else if (book.AvailabilityStatus.Name == "UNKNOWN_CONDITION")
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "The item is in unknown whereabouts and condition and cannot be reserved!");
                return View(reservationModel);
            }
            else if (book.AvailabilityStatus.Name == "DESTROYED")
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "The item has been destroyed and cannot be reserved!");
                return View(reservationModel);
            }

            else 
            {
                Reservation reservation = new Reservation();

                reservation.Book = book;
                reservation.LibraryCard = patron.LibraryCard;
                reservation.HoldPlaced = reservation.HoldPlaced;
                reservation.UpdatedOn = DateTime.Now;
                reservation.Status = reservationModel.Status;

                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Basic, Admin")]
        public async Task<IActionResult> Reserve(ReservationFullModel reservationModel)
        {
            var patron = await _context.Patrons.Include(p => p.LibraryCard).Include(p => p.HomeLibraryBranch).FirstOrDefaultAsync(p => p.Email == reservationModel.Patron.Email);
            var book = await _context.Books.Include(b => b.AvailabilityStatus).Include(b => b.Location).FirstOrDefaultAsync(b => b.Id == reservationModel.Book.Id);
            var libraryCard = await _context.LibraryCards.Include(l => l.Checkouts).ThenInclude(c => c.Book).FirstOrDefaultAsync(l => l.Id == patron.LibraryCard.Id);


            var reservations = await _context.Reservation.Include(r => r.Book).Include(r => r.LibraryCard).FirstOrDefaultAsync(r => r.LibraryCard.Id == libraryCard.Id && r.Book.Id == book.Id);
            var reservationM = new ReservationFullModel
            {
                Book = book,
                LibraryCard = libraryCard,
                Patron = patron,
            };
            if (patron == null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This email is not registered as a user and patron!");
                return View("~/Views/Books/Reservation.cshtml", reservationM);
            };

            if (patron.AccountStatus == "Pending" || patron.AccountStatus == "Deactivated")
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "This patron does not have an approved account!");
                return View("~/Views/Books/Reservation.cshtml", reservationM);

            };

            foreach (var checkoutItem in libraryCard.Checkouts)
            {
                if (checkoutItem.Book == book)
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item has already been checked out on this library card!");
                    return View("~/Views/Books/Reservation.cshtml", reservationM);
                }
            }

            if (reservations != null)
            {
                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Danger, "Item has already been reserved on this library card!");
                return View("~/Views/Books/Reservation.cshtml", reservationM);
            }

            else
            {
                Reservation reservation = new Reservation();

                reservation.Book = book;
                reservation.LibraryCard = patron.LibraryCard;
                reservation.HoldPlaced = DateTime.Now;
                reservation.UpdatedOn = DateTime.Now;
                reservation.Status = "Active";

                _context.Add(reservation);
                await _context.SaveChangesAsync();

                ViewBag.Alert = CommonServices.ShowAlert(Alerts.Success, "Item has successfully been reserved by you!");
                return View("~/Views/Books/Reservation.cshtml", reservationM);
            }

        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.Include(c => c.Book).Include(c => c.LibraryCard)
                .FirstOrDefaultAsync(m => m.Id == id);

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == reservation.LibraryCard.Id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationModel = new ReservationFullModel
            {
                Id = reservation.Id,
                Patron = patron,
                LibraryCard = reservation.LibraryCard,
                HoldPlaced = reservation.HoldPlaced,
                UpdatedOn = reservation.UpdatedOn,
                Book = reservation.Book,
                Status = reservation.Status,
            };

            return View(reservationModel);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ReservationFullModel reservationModel)
        {
            if (id != reservationModel.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{

            Reservation reservation = new Reservation()
            {
                Id = reservationModel.Id,
                LibraryCard = reservationModel.LibraryCard,
                Book = reservationModel.Book,
                HoldPlaced = reservationModel.HoldPlaced,
                UpdatedOn = reservationModel.UpdatedOn,
                Status = reservationModel.Status,
            };
            try
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservation.Id))
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
            //return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.Include(c => c.Book).Include(c => c.LibraryCard)
                .FirstOrDefaultAsync(m => m.Id == id);

            var patron = await _context.Patrons.Include(p => p.LibraryCard).FirstOrDefaultAsync(p => p.LibraryCard.Id == reservation.LibraryCard.Id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationModel = new ReservationFullModel
            {
                Id = reservation.Id,
                Book = reservation.Book,
                LibraryCard = reservation.LibraryCard,
                Patron = patron,
                HoldPlaced = reservation.HoldPlaced,
                UpdatedOn = reservation.UpdatedOn,
                Status = reservation.Status
            };

            return View(reservationModel);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            _context.Reservation.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }
    }
}
