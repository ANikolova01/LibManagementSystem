using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class BranchHoursModel
    {
        public LibraryBranch Branch { get; set; }
        public IEnumerable<BranchHours> BranchHours { get; set; }
    }
}
