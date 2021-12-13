using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryMSystem.Data.Models;
using LibraryManagementSystem.Data;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.BranchHours.ToListAsync());
        }

        // GET: BranchHours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branchHours = await _context.BranchHours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branchHours == null)
            {
                return NotFound();
            }

            return View(branchHours);
        }

        // GET: BranchHours/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BranchHours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DayOfWeek,OpenTime,CloseTime")] BranchHours branchHours)
        {
            if (ModelState.IsValid)
            {
                var Branch = await _context.LibraryBranches.FirstOrDefaultAsync(b => b.Name == branchHours.Branch.Name);
                branchHours.Branch = Branch;
                _context.Add(branchHours);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(branchHours);
        }

        // GET: BranchHours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branchHours = await _context.BranchHours.FindAsync(id);
            if (branchHours == null)
            {
                return NotFound();
            }
            return View(branchHours);
        }

        // POST: BranchHours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DayOfWeek,OpenTime,CloseTime")] BranchHours branchHours)
        {
            if (id != branchHours.Id)
            {
                return NotFound();
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
                        return NotFound();
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
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var branchHours = await _context.BranchHours
                .FirstOrDefaultAsync(m => m.Id == id);
            if (branchHours == null)
            {
                return NotFound();
            }

            return View(branchHours);
        }

        // POST: BranchHours/Delete/5
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
