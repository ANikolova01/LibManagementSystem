using LibraryManagementSystem.Interfaces;
using LibraryManagementSystem.Data;
using System.Threading.Tasks;
using LibraryMSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace LibraryManagementSystem.Services
{
    public class LibraryBranchService : ILibraryBranchService
    {
        private readonly LibraryDbContext _context;

        public LibraryBranchService(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<LibraryBranch> Get(int branchId)
        {
            var branches = await _context.LibraryBranches.Include(b => b.Patrons).Include(b => b.LibraryBooks).FirstAsync(p => p.Id == branchId);

            return branches;
        }

        public async Task<IEnumerable<Book>> GetBooks(int branchId)
        {
            var booksInBranch = await _context.LibraryBranches.Include(a => a.LibraryBooks)
                .FirstOrDefaultAsync(b => b.Id == branchId);

            return booksInBranch.LibraryBooks;
        }

        public async Task<int> GetBooksCount(int branchId)
        {
            var libraryBranch = await Get(branchId);
            return libraryBranch.LibraryBooks.Count();
        }

        public async Task<decimal> GetBooksValue(int branchId)
        {
            var booksInBranch = await _context.LibraryBranches.Include(b => b.LibraryBooks).FirstOrDefaultAsync(b => b.Id == branchId);
            return booksInBranch.TotalAssetValue;
        }



        public async Task<int> GetPatronNumber(int branchId)
        {
            var libraryBranch = await Get(branchId);
            return libraryBranch.Patrons.Count();
        }

        public async Task<bool> UpdateAssetAmount(int branchId, int newAmount)
        {
            var branch = await _context.LibraryBranches.SingleOrDefaultAsync(b => b.Id == branchId);
            if (branch != null)
            {
                branch.TotalAssetValue = branch.TotalAssetValue + newAmount;
                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> UpdateAssetsCount(int branchId, int newBook)
        {
            var branch = await Get(branchId);
            if (branch != null)
            {
                branch.NumberOfAssets = branch.NumberOfAssets + newBook;
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdatePatronCount(int branchId, int patronNumber)
        {
            var branch = await Get(branchId);
            if (branch != null)
            {
                branch.NumberOfPatrons = branch.NumberOfAssets + patronNumber;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
