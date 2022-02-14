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
using LibraryManagementSystem.Models.Enums;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class LibraryBranchesController : Controller
    {
        private readonly LibraryDbContext _context;

        public LibraryBranchesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: LibraryBranches
        public async Task<IActionResult> Index()
        {
            return View(await _context.LibraryBranches.ToListAsync());
        }

        // GET: LibraryBranches
        public async Task<IActionResult> IndexBasic()
        {
            return View(await _context.LibraryBranches.ToListAsync());
        }

        // GET: LibraryBranches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryBranch = await _context.LibraryBranches.FirstOrDefaultAsync(m => m.Id == id);
            var branchHours = await _context.BranchHours.Include(b => b.Branch).ToListAsync();
            var branchHoursLib = new List<BranchHours>();

            foreach(BranchHours branchHour in branchHours)
            {
                if(branchHour.Branch.Id == id)
                {
                    branchHoursLib.Add(branchHour);
                }
            }

            var patronsCount = await _context.Patrons.CountAsync();

            libraryBranch.NumberOfPatrons = patronsCount;
            if (libraryBranch == null)
            {
                return NotFound();
            }
            if(patronsCount > libraryBranch.NumberOfPatrons)
            {
                _context.Update(libraryBranch);
                await _context.SaveChangesAsync();
            }
            var branchHoursModel = new BranchHoursModel
            {
                Branch = libraryBranch,
                BranchHours = branchHoursLib
            };
            this.addBranchHoursBanner(branchHoursModel);
            return View(branchHoursModel);
        }
        public void addBranchHoursBanner(BranchHoursModel branchHoursModel)
        {
            var dayCurrent = ((int)DateTime.Now.DayOfWeek);
            if(branchHoursModel.BranchHours != null)
            {
            var workingDayCheck = branchHoursModel.BranchHours.FirstOrDefault(b => b.DayOfWeek == (dayCurrent - 1));
            if(workingDayCheck != null)
                {
                var timeNow = DateTime.Now.Hour;
                if(timeNow > workingDayCheck.OpenTime && timeNow < workingDayCheck.CloseTime)
                { ViewBag.Alert = CommonServices.ShowAlert(Alerts.Success, "This library branch is currently open!");  }
                 else ViewBag.Alert = CommonServices.ShowAlert(Alerts.Warning, "This Library branch is currently closed!");
                }
                else
                {
                    ViewBag.Alert = CommonServices.ShowAlert(Alerts.Warning, "This Library branch is currently closed!");
                }
            }

        }

        // GET: LibraryBranches/Create
        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: LibraryBranches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Address,Telephone,Description,OpenDate,NumberOfPatrons,NumberOfAssets,TotalAssetValue,BranchImage")] LibraryBranch libraryBranch)
        {
            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    libraryBranch.BranchImage = dataStream.ToArray();
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(libraryBranch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libraryBranch);
        }

        // GET: LibraryBranches/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryBranch = await _context.LibraryBranches.FindAsync(id);
            if (libraryBranch == null)
            {
                return NotFound();
            }
            return View(libraryBranch);
        }

        // POST: LibraryBranches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address,Telephone,Description,OpenDate,NumberOfPatrons,NumberOfAssets,TotalAssetValue,BranchImage")] LibraryBranch libraryBranch)
        {
            if (id != libraryBranch.Id)
            {
                return NotFound();
            }

            var oldBranch = await _context.LibraryBranches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == libraryBranch.Id);

            if (Request.Form.Files.Count > 0)
            {
                IFormFile file = Request.Form.Files.FirstOrDefault();
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);
                    libraryBranch.BranchImage = dataStream.ToArray();
                }
            }

            else
            {
                libraryBranch.BranchImage = oldBranch.BranchImage;
            }

            LibraryBranch updatedBranch = libraryBranch;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(updatedBranch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibraryBranchExists(libraryBranch.Id))
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
            return View(libraryBranch);
        }

        // GET: LibraryBranches/Delete/5
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var libraryBranch = await _context.LibraryBranches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (libraryBranch == null)
            {
                return NotFound();
            }

            return View(libraryBranch);
        }

        // POST: LibraryBranches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libraryBranch = await _context.LibraryBranches.FindAsync(id);
            _context.LibraryBranches.Remove(libraryBranch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibraryBranchExists(int id)
        {
            return _context.LibraryBranches.Any(e => e.Id == id);
        }
    }
}
