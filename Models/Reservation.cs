using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    [Table("Reservations")]
    public class Reservation
    {
        [Display(Name = "Hold placed on date")]
        public DateTime HoldPlaced { get; set; }
        [Display(Name = "Updated On")]
        public DateTime UpdatedOn { get; set; }
        public int Id { get; set; }
        public virtual Book Book { get; set; }
        public virtual LibraryCard LibraryCard { get; set; }
        public string Status { get; set; } //if it has been aquired or not by the Library Card holder
    }
}
