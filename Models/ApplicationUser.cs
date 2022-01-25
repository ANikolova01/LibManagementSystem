using Microsoft.AspNetCore.Identity;
using System;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int UsernameChangeLimit { get; set; } = 10;
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string HomeLibraryBranch { get; set; }
        public Patron PatronAcc { get; set; }
    }
}
