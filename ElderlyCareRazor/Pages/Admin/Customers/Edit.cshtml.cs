using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ElderlyCareRazor.Pages.Admin.Customers
{
    public class EditModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IElderService _elderService;
        private readonly IBookingService _bookingService;
        private readonly IFeedbackService _feedbackService;
        private readonly IRoleService _roleService;

        public EditModel(
            IAccountService accountService,
            IElderService elderService,
            IBookingService bookingService,
            IFeedbackService feedbackService,
            IRoleService roleService)
        {
            _accountService = accountService;
            _elderService = elderService;
            _bookingService = bookingService;
            _feedbackService = feedbackService;
            _roleService = roleService;
        }

        [BindProperty]
        public AccountEditModel Input { get; set; } = new AccountEditModel();

        [BindProperty]
        public bool ChangePassword { get; set; } = false;

        public List<Elder> Elders { get; set; }
        public int BookingsCount { get; set; }
        public int CompletedBookingsCount { get; set; }
        public int FeedbacksCount { get; set; }
        public SelectList StatusOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Check if user is logged in as admin
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Admin")
            {
                return RedirectToPage("/Auth/Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            // Get customer account information
            var account = await _accountService.GetAccountByIdAsync(id.Value);
            if (account == null)
            {
                return NotFound();
            }

            // Only allow editing customer accounts (role ID 2)
            var customerRoleId = (await _roleService.GetAllRolesAsync())
                .FirstOrDefault(r => r.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))?.RoleId ?? 2;

            if (account.RoleId != customerRoleId)
            {
                TempData["ErrorMessage"] = "Only customer accounts can be edited in this section.";
                return RedirectToPage("/Admin/Customers");
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

            // Get elders associated with this customer
            Elders = _elderService.GetEldersByAccountId(id.Value);

            // Get activity statistics
            var bookings = _bookingService.GetBookingsByAccountId(id.Value);
            BookingsCount = bookings?.Count ?? 0;
            CompletedBookingsCount = bookings?.Count(b => b.Status == "completed") ?? 0;

            try
            {
                var feedbacks = _feedbackService.GetFeedbacksByCustomerId(id.Value);
                FeedbacksCount = feedbacks?.Count ?? 0;
            }
            catch
            {
                FeedbacksCount = 0;
            }

            await LoadDropdownDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if user is logged in as admin
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Admin")
            {
                return RedirectToPage("/Auth/Login");
            }

            // Handle password validation only if changing password is checked
            if (!ChangePassword)
            {
                ModelState.Remove("Input.NewPassword");
                ModelState.Remove("Input.ConfirmNewPassword");
            }

            if (!ModelState.IsValid)
            {
                // Reload related data
                Elders = _elderService.GetEldersByAccountId(Input.AccountId);
                var bookings = _bookingService.GetBookingsByAccountId(Input.AccountId);
                BookingsCount = bookings?.Count ?? 0;
                CompletedBookingsCount = bookings?.Count(b => b.Status == "completed") ?? 0;
                try
                {
                    var feedbacks = _feedbackService.GetFeedbacksByCustomerId(Input.AccountId);
                    FeedbacksCount = feedbacks?.Count ?? 0;
                }
                catch
                {
                    FeedbacksCount = 0;
                }
                await LoadDropdownDataAsync();
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
                account.AccountStatus = Input.AccountStatus;

                // Username and RoleId should not change for existing customers
                // account.Username = Input.Username; 
                // account.RoleId = Input.RoleId;

                // Set password to empty string by default to keep existing password
                account.Password = string.Empty;

                // If changing password and a new password was provided
                if (ChangePassword && !string.IsNullOrWhiteSpace(Input.NewPassword))
                {
                    // Password will be hashed in the service
                    account.Password = Input.NewPassword;
                }

                await _accountService.UpdateAccountAsync(account);

                TempData["SuccessMessage"] = "Customer account updated successfully";
                return RedirectToPage("Details", new { id = Input.AccountId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating customer account: {ex.Message}";
                // Reload related data
                Elders = _elderService.GetEldersByAccountId(Input.AccountId);
                await LoadDropdownDataAsync();
                return Page();
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            // Setup status options
            var statusOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "active", Text = "Active" },
                new SelectListItem { Value = "inactive", Text = "Inactive" }
            };
            StatusOptions = new SelectList(statusOptions, "Value", "Text");
        }
    }

    public class AccountEditModel
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

        [Required]
        [Display(Name = "Account Status")]
        public string AccountStatus { get; set; } = null!;

        [Required]
        [Display(Name = "Role")]
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