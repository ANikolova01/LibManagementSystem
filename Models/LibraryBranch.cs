using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    [Table("LibraryBranches")]
    public class LibraryBranch
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name of Library Branch")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Date of Opening")]
        public DateTime OpenDate { get; set; }
        [Display(Name = "Number of Patrons")]
        public int NumberOfPatrons { get; set; }
        [Display(Name = "Number of Assets")]
        public int NumberOfAssets { get; set; }
        [Display(Name = "Total Asset Value")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal TotalAssetValue { get; set; }
        public virtual IEnumerable<Patron> Patrons { get; set; }
        public virtual IEnumerable<Book> LibraryBooks { get; set; }

        [Display(Name = "Library Branch Photo")]
        public byte[] BranchImage { get; set; }

    }
}
