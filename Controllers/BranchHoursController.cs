using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class BranchHoursController : Controller
    {
        private readonly LibraryDbContext _context;

        public BranchHoursController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: BranchHours
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.BranchHours.Include(h => h.Branch).ToListAsync());
        }

        // GET: BranchHours/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("~/Views/BranchHours/NotFound.cshtml");
            }

            var branchHours = await _context.BranchHours.Include(h => h.Branch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branchHours == null)
            {
                return View("~/Views/BranchHours/NotFound.cshtml");
            }

            return View(branchHours);
        }

        // GET: BranchHours/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: BranchHours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BranchHours branchHours)
        {
            //if (ModelState.IsValid)
            //{
                var Branch = await _context.LibraryBranches.FirstOrDefaultAsync(b => b.Name == branchHours.Branch.Name);
                branchHours.Branch = Branch;
                BranchHours branchHoursModel = branchHours;
                _context.Add(branchHoursModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            //}
            //return View(branchHours);
        }

        // GET: BranchHours/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("~/Views/BranchHours/NotFound.cshtml");
            }

            var branchHours = await _context.BranchHours.Include(h => h.Branch).FirstOrDefaultAsync(h => h.Id == id);
            if (branchHours == null)
            {
                return View("~/Views/BranchHours/NotFound.cshtml");
            }
            return View(branchHours);
        }

        // POST: BranchHours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DayOfWeek,OpenTime,CloseTime")] BranchHours branchHours)
        {
            if (id != branchHours.Id)
            {
                return View("~/Views/BranchHours/NotFound.cshtml");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(branchHours);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BranchHoursExists(branchHours.Id))
                    {
                        return View("~/Views/BranchHours/NotFound.cshtml");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(branchHours);
        }

        // GET: BranchHours/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("~/Views/BranchHours/NotFound.cshtml");
            }

            var branchHours = await _context.BranchHours.Include(h => h.Branch)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branchHours == null)
            {
                return View("~/Views/BranchHours/NotFound.cshtml");
            }

            return View(branchHours);
        }

        // POST: BranchHours/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var branchHours = await _context.BranchHours.FindAsync(id);
            _context.BranchHours.Remove(branchHours);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BranchHoursExists(int id)
        {
            return _context.BranchHours.Any(e => e.Id == id);
        }
    }
}
