using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    [Table("LibraryCards")]
    public class LibraryCard
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Current Fees")]
        public decimal CurrentFees { get; set; }
        [Display(Name = "Issued")]
        public DateTime Issued { get; set; }
        public virtual IEnumerable<Checkout> Checkouts { get; set; }
    }
}
