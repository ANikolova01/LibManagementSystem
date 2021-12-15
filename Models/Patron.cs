using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    [Table("Patrons")]
    public class Patron
    {
        public int Id { get; set; }
        [Display(Name = "Created On")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }
        [Required] public LibraryCard LibraryCard { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Overdue Fees")]
        public decimal OverdueFees { get; set; }
        public LibraryBranch HomeLibraryBranch { get; set; }
        [Display(Name = "Account Status")]
        public string AccountStatus { get; set; }
    }
}
