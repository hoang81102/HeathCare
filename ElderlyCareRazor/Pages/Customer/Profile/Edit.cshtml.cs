using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using Services;

namespace ElderlyCareRazor.Pages.Customer.Profile
{
    public class EditModel : PageModel
    {
        private readonly IAccountService _accountService;

        public EditModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty]
        public CustomerProfileEditModel Input { get; set; } = new CustomerProfileEditModel();

        [BindProperty]
        public bool ChangePassword { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            // Get current user ID from session
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            var account = await _accountService.GetAccountByIdAsync(accountId.Value);
            if (account == null)
            {
                return NotFound();
            }

            // Map account data to the input model
            Input.AccountId = account.AccountId;
            Input.Username = account.Username;
            Input.Email = account.Email;
            Input.Phone = account.Phone;
            Input.Fullname = account.Fullname;
            Input.Address = account.Address;
            Input.Birthdate = account.Birthdate;
            Input.Hobby = account.Hobby;
            Input.AccountStatus = account.AccountStatus;
            Input.RoleId = account.RoleId;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remove validation errors for password fields if we're not changing the password
            if (!ChangePassword)
            {
                ModelState.Remove("Input.NewPassword");
                ModelState.Remove("Input.ConfirmNewPassword");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Get existing account
                var account = await _accountService.GetAccountByIdAsync(Input.AccountId);
                if (account == null)
                {
                    return NotFound();
                }



                // Update account details
                account.Email = Input.Email;
                account.Phone = Input.Phone;
                account.Fullname = Input.Fullname;
                account.Address = Input.Address;
                account.Birthdate = Input.Birthdate;
                account.Hobby = Input.Hobby;

                // Set password to empty string by default to keep existing password
                account.Password = string.Empty;

                // If changing password and a new password was provided
                if (ChangePassword && !string.IsNullOrWhiteSpace(Input.NewPassword))
                {
                    // No need to hash here as it will be handled by UpdateAccountAsync
                    account.Password = Input.NewPassword;
                }

                await _accountService.UpdateAccountAsync(account);

                TempData["SuccessMessage"] = "Your profile was updated successfully";
                return RedirectToPage("/Customer/Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating profile: {ex.Message}");
                return Page();
            }
        }
    }

    public class CustomerProfileEditModel
    {
        public int AccountId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [StringLength(15)]
        [Phone]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Full Name")]
        public string Fullname { get; set; } = null!;

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateOnly? Birthdate { get; set; }

        [Display(Name = "Hobbies")]
        public string? Hobby { get; set; }

        // Hidden fields - not editable by the user
        public string AccountStatus { get; set; } = null!;
        public int RoleId { get; set; }

        // Password fields - only used when changing password
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmNewPassword { get; set; }
    }
}