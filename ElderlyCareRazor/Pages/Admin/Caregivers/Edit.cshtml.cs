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

namespace ElderlyCareRazor.Pages.Admin.Caregivers
{
    public class EditModel : PageModel
    {
        private readonly ICaregiverService _caregiverService;
        private readonly IAccountService _accountService;
        private readonly ICaregiverAvailabilityService _availabilityService;

        public EditModel(
            ICaregiverService caregiverService,
            IAccountService accountService,
            ICaregiverAvailabilityService availabilityService)
        {
            _caregiverService = caregiverService;
            _accountService = accountService;
            _availabilityService = availabilityService;
        }

        [BindProperty]
        public AccountEditModel Input { get; set; } = new AccountEditModel();

        [BindProperty]
        public CaregiverEditModel CaregiverInput { get; set; } = new CaregiverEditModel();

        [BindProperty]
        public bool ChangePassword { get; set; } = false;

        public List<CaregiverAvailability> Availabilities { get; set; }
        public SelectList AvailabilityOptions { get; set; } = default!;
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

            // Get caregiver details
            var caregiver = _caregiverService.GetCaregiverById(id.Value);
            if (caregiver == null)
            {
                return NotFound();
            }

            // Get account information
            var account = await _accountService.GetAccountByIdAsync(caregiver.AccountId);
            if (account == null)
            {
                TempData["ErrorMessage"] = "Account information not found.";
                return RedirectToPage("/Admin/Caregivers");
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

            // Map caregiver data to the input model
            CaregiverInput.CaregiverId = caregiver.CaregiverId;
            CaregiverInput.ExperienceYears = caregiver.ExperienceYears;
            CaregiverInput.Specialty = caregiver.Specialty;
            CaregiverInput.Certification = caregiver.Certification;
            CaregiverInput.Availability = caregiver.Availability;

            // Get availability schedule
            Availabilities = _availabilityService.GetAvailabilitiesByCaregiverId(id.Value);

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
                // Reload availability schedule
                Availabilities = _availabilityService.GetAvailabilitiesByCaregiverId(CaregiverInput.CaregiverId);
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

                // Username and RoleId should not change for existing caregivers
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

                // Get existing caregiver
                var caregiver = _caregiverService.GetCaregiverById(CaregiverInput.CaregiverId);
                if (caregiver == null)
                {
                    return NotFound();
                }

                // Update caregiver details
                caregiver.ExperienceYears = CaregiverInput.ExperienceYears;
                caregiver.Specialty = CaregiverInput.Specialty;
                caregiver.Certification = CaregiverInput.Certification;
                caregiver.Availability = CaregiverInput.Availability;

                _caregiverService.UpdateCaregiver(caregiver);

                TempData["SuccessMessage"] = "Caregiver updated successfully";
                return RedirectToPage("Details", new { id = CaregiverInput.CaregiverId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating caregiver: {ex.Message}";
                // Reload availability schedule
                Availabilities = _availabilityService.GetAvailabilitiesByCaregiverId(CaregiverInput.CaregiverId);
                await LoadDropdownDataAsync();
                return Page();
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            // Setup availability options for caregivers
            var availabilityOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "full-time", Text = "Full Time" },
                new SelectListItem { Value = "part-time", Text = "Part Time" },
                new SelectListItem { Value = "on-call", Text = "On Call" }
            };
            AvailabilityOptions = new SelectList(availabilityOptions, "Value", "Text");

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

    public class CaregiverEditModel
    {
        public int CaregiverId { get; set; }

        [Required]
        [Display(Name = "Years of Experience")]
        [Range(0, 50, ErrorMessage = "Experience years must be between 0 and 50")]
        public int ExperienceYears { get; set; }

        [Display(Name = "Specialty")]
        public string? Specialty { get; set; }

        [Display(Name = "Certification")]
        public string? Certification { get; set; }

        [Required]
        [Display(Name = "Availability")]
        public string Availability { get; set; } = null!;
    }
}