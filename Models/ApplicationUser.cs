using Microsoft.AspNetCore.Identity;
using System;
using LibraryMSystem.Data.Models;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public string Address { get; set; }
        public string HomeLibraryBranch { get; set; }
        public Patron PatronAcc { get; set; }
    }
}
