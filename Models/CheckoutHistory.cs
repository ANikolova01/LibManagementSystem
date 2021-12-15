using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    [Table("CheckoutHistories")]
    public class CheckoutHistory
    {
        public int Id { get; set; }

        [Required]
        public Book Book { get; set; }

        [Required]
        public LibraryCard LibraryCard { get; set; }

        [Required]
        [Display(Name = "Checked Out")]
        public DateTime CheckedOut { get; set; }
        [Display(Name = "Checked In")]
        public DateTime? CheckedIn { get; set; }
    }
}
