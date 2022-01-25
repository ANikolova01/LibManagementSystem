using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class CheckoutFullModel
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public LibraryCard LibraryCard { get; set; }
        [Display(Name = "Checked out Since")]
        public Patron Patron { get; set; }
        public DateTime CheckedOutSince { get; set; }
        [Display(Name = "Checked out Until")]
        public DateTime CheckedOutUntil { get; set; }
    }
}
