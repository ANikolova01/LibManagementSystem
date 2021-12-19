using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    [Table("Books")]
    public class Book
    {
        [Key] public Guid Id { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Author { get; set; }
        [Required] public string ISBN { get; set; }
        [Display(Name = "Publication Year")]
        public int PublicationYear { get; set; }
        [Display(Name = "Edition")]
        public string Edition { get; set; }
        [Display(Name = "Publisher")]
        public string Publisher { get; set; }
        [Display(Name = "Dewey Index")]
        public string DeweyIndex { get; set; }
        [Display(Name = "Language")]
        public string Language { get; set; }
        [Display(Name = "Number of Pages/Length time (minutes)")]
        public int NoOfPages_LengthTime { get; set; }
        [Display(Name = "Genre")]
        public string Genre { get; set; }
        [Display(Name = "Summary")]
        public string Summary { get; set; }
        [Display(Name = "Book Cover")]
        public byte[] BookImage { get; set; }
        public string AvailabilityStatus { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Cost { get; set; }
        public LibraryBranch Location { get; set; }
        public int NumberOfCopies { get; set; }
        public int CopiesAvailable { get; set; }
        public string Type { get; set; }
    }
}