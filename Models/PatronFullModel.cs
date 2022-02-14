using System.Collections.Generic;

namespace LibraryManagementSystem.Models
{
    public class PatronFullModel
    {
        public Patron Patron { get; set; }
        public IEnumerable<CheckoutHistory> CheckoutHistory { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }

    }
}
