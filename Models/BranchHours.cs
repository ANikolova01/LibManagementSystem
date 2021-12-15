using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    [Table("BranchHours")]
    public class BranchHours
    {
        public int Id { get; set; }
        public LibraryBranch Branch { get; set; }

        [Range(0, 6, ErrorMessage = "Day of Week must be between 0 and 6")]
        [Display(Name = "Day of Week")]
        public int DayOfWeek { get; set; }

        [Range(0, 23, ErrorMessage = "Hour open must be between 0 and 23")]
        [Display(Name = "Opening Time")]
        public int OpenTime { get; set; }

        [Range(0, 23, ErrorMessage = "Hour closed must be between 0 and 23")]
        [Display(Name = "Closing Time")]
        public int CloseTime { get; set; }
    }
}
