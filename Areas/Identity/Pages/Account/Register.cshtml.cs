using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using LibraryManagementSystem.Models.Enums;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly LibraryDbContext _context;


        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            LibraryDbContext libraryContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = libraryContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }
            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Username")]
            public string UserName { get; set; }
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
     //       [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$", ErrorMessage ="Password must be at least 8 characters long and have at least one letter, one number and one special character.")]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Required(ErrorMessage = "Confirmation Password is required.")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Home Library Branch")]
            public string HomeLibraryBranch { get; set; }

            [Display(Name = "Date of Birth")]
            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            [Display(Name = "Address")]
            [Required]
            public string Address { get; set; }

            [Phone]
            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var userCheck = await _userManager.FindByNameAsync(Input.UserName);
            var mailCheck = await _userManager.FindByEmailAsync(Input.Email);
            if(userCheck != null)
            {
                StatusMessage = "Username is already in use. Select a different username.";
                return RedirectToPage();
            }
            else if(mailCheck != null) {
                StatusMessage = "Email is already in use. Select a different email.";
                return RedirectToPage();
            }
            if (ModelState.IsValid)
            {
                var branch = await _context.LibraryBranches.FirstOrDefaultAsync(b => b.Name == Input.HomeLibraryBranch);

                LibraryBranch branchDraft = branch;

                var patronDraft = new Patron
                {
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    Address = Input.Address,
                    DateOfBirth = Input.DateOfBirth,
                    Email = Input.Email,
                    Telephone = Input.PhoneNumber,
                    OverdueFees = 10,
                    HomeLibraryBranch = branchDraft,
                    AccountStatus = "Pending",

                };

                //   Patron patron = patronDraft;

                patronDraft.Id = new Guid();
                _context.Patrons.Add(patronDraft);
                await _context.SaveChangesAsync();

                //LibraryBranch branchDraft1 = branchDraft;
                //patronDraft.HomeLibraryBranch = branchDraft1;

                Patron patronDraft1 = patronDraft;


                MailAddress address = new MailAddress(Input.Email);
                string userName = address.User;
                var user = new ApplicationUser { 
                    UserName = Input.UserName, 
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    HomeLibraryBranch = Input.HomeLibraryBranch,
                    DateOfBirth = Input.DateOfBirth,
                    Address = Input.Address
                };

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, Roles.Basic.ToString());

                    user.PatronAcc = patronDraft;

                    await _userManager.UpdateAsync(user);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
