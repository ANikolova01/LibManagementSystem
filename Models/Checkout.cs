using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    [Table("Checkouts")]
    public class Checkout
    {
        public int Id { get; set; }
        [Required] public Book Book { get; set; }
        public LibraryCard LibraryCard { get; set; }
        [Display(Name = "Checked out Since")]
        public DateTime CheckedOutSince { get; set; }
        [Display(Name = "Checked out Until")]
        public DateTime CheckedOutUntil { get; set; }
    }
}
