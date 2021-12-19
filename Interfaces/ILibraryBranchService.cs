using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;


namespace LibraryManagementSystem.Interfaces
{
    public interface ILibraryBranchService
    {
        Task<LibraryBranch> Get(int branchId);
        Task<bool> UpdatePatronCount(int branchId, int patronNumber);
        Task<bool> UpdateAssetsCount(int branchId, int newAssets);
        Task<bool> UpdateAssetAmount(int branchId, decimal newAmount);

        Task<int> GetBooksCount(int branchId);
        Task<int> GetPatronNumber(int branchId);
        Task<decimal> GetBooksValue(int branchId);
    }
}
