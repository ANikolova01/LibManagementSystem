using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class CheckoutHistoryFullModel
    {
        public int Id { get; set; }
        public Book Book { get; set; }
        public LibraryCard LibraryCard { get; set; }
        [Display(Name = "Checked Out")]
        public Patron Patron { get; set; }
        public DateTime CheckedOut { get; set; }
        [Display(Name = "Checked In")]
        public DateTime? CheckedIn { get; set; }
    }
}
