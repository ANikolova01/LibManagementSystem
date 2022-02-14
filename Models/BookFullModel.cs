using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BookFullModel
    {
        public Book Book { get; set; }
        public IEnumerable<Checkout> Checkouts { get; set; }
        public IEnumerable<CheckoutHistory> History { get; set; }   
        public IEnumerable<Reservation> Reservations { get; set; }
    }
}
