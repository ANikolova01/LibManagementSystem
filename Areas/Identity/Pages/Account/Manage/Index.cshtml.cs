using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly LibraryDbContext _context;
        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            LibraryDbContext libraryDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = libraryDbContext;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public string UserNameChangeLimitMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Display(Name = "Last Name")]
            public string LastName { get; set; }
            [Display(Name = "Username")]
            public string Username { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Display(Name = "Date Of Birth")]
            public DateTime DateOfBirth { get; set; }
            [Display(Name = "Address")]
            public string Address { get; set; }
            [Display(Name = "Home Library Branch")]
            public string HomeLibraryBranch { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var dateOfBirth = user.DateOfBirth;
            var Address = user.Address;
            var HomeLibraryBranch = user.HomeLibraryBranch;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Username = userName,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                Address = Address,
                HomeLibraryBranch = HomeLibraryBranch
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            UserNameChangeLimitMessage = $"You can change your username {user.UsernameChangeLimit} more time(s).";

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var firstName = user.FirstName;
            var lastName = user.LastName;
            var dateOfBirth = user.DateOfBirth;
            var address = user.Address;
            var homeLibraryBranch = user.HomeLibraryBranch;

            var user1 = await _userManager.Users.Include(u => u.PatronAcc).FirstOrDefaultAsync(u => u.Id == user.Id);

            if (Input.FirstName != firstName)
            {
                user.FirstName = Input.FirstName;
                await _userManager.UpdateAsync(user);
                if(roles[0] == "Basic")
                {
                    var patronAcc = user1.PatronAcc;
                    if (patronAcc != null)
                    {
                        patronAcc.FirstName = Input.FirstName;
                        patronAcc.UpdatedOn = DateTime.Now;
                        _context.Update(patronAcc);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            if (Input.LastName != lastName)
            {
                user.LastName = Input.LastName;
                await _userManager.UpdateAsync(user);
                if (roles[0] == "Basic")
                {
                    var patronAcc = user1.PatronAcc;
                    if (patronAcc != null)
                    {
                        patronAcc.LastName = Input.LastName;
                        patronAcc.UpdatedOn = DateTime.Now;
                        _context.Update(patronAcc);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            if (Input.DateOfBirth != dateOfBirth)
            {
                user.DateOfBirth = Input.DateOfBirth;
                await _userManager.UpdateAsync(user);
                if (roles[0] == "Basic")
                {
                    var patronAcc = user1.PatronAcc;
                    if (patronAcc != null)
                    {
                        patronAcc.DateOfBirth = Input.DateOfBirth;
                        patronAcc.UpdatedOn = DateTime.Now;
                        _context.Update(patronAcc);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            if (Input.Address != address)
            {
                user.Address = Input.Address;
                await _userManager.UpdateAsync(user);
                if (roles[0] == "Basic")
                {
                    var patronAcc = user1.PatronAcc;
                    if (patronAcc != null)
                    {
                        patronAcc.Address = Input.Address;
                        patronAcc.UpdatedOn = DateTime.Now;
                        _context.Update(patronAcc);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            // update code for updating Patron account at the same time for any users
           

            if (Input.HomeLibraryBranch != homeLibraryBranch)
            {
                user.HomeLibraryBranch = Input.HomeLibraryBranch;
                await _userManager.UpdateAsync(user);
                if (roles[0] == "Basic")
                {
                    var patronAcc = user1.PatronAcc;
                    var branch = await _context.LibraryBranches.AsNoTracking().FirstOrDefaultAsync(b => b.Name == Input.HomeLibraryBranch);
                    if (patronAcc != null)
                    {
                        patronAcc.HomeLibraryBranch = branch;
                        patronAcc.UpdatedOn = DateTime.Now;
                        var patronModel = patronAcc;
                        _context.Update(patronModel);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (roles[0] == "Basic")
                {
                    var patronAcc = await _context.Patrons.FirstOrDefaultAsync(p => p.Id == user.PatronAcc.Id);
                    if (patronAcc != null)
                    {
                        patronAcc.Telephone = Input.PhoneNumber;
                        patronAcc.UpdatedOn = DateTime.Now;
                        _context.Update(patronAcc);
                        await _context.SaveChangesAsync();
                    }
                }
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            if (user.UsernameChangeLimit > 0)
            {
                if (Input.Username != user.UserName)
                {
                    var userNameExists = await _userManager.FindByNameAsync(Input.Username);
                    if (userNameExists != null)
                    {
                        StatusMessage = "User name already taken. Select a different username.";
                        return RedirectToPage();
                    }
                    var setUserName = await _userManager.SetUserNameAsync(user, Input.Username);
                    if (!setUserName.Succeeded)
                    {
                        StatusMessage = "Unexpected error when trying to set user name.";
                        return RedirectToPage();
                    }
                    else
                    {
                        user.UsernameChangeLimit -= 1;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
