using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class ReservationFullModel
    {
        [Display(Name = "Hold placed on date")]
        public DateTime HoldPlaced { get; set; }
        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }
        public int Id { get; set; }
        public Book Book { get; set; }
        public LibraryCard LibraryCard { get; set; }
        public Patron Patron { get; set; }
        public string Status { get; set; } //if it has been aquired or not by the Library Card holder
    }
}
