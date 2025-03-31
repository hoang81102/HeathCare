using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Caregiver.Profile
{
    public class EditModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ICaregiverService _caregiverService;

        public EditModel(IAccountService accountService, ICaregiverService caregiverService)
        {
            _accountService = accountService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public CaregiverProfileModel Input { get; set; } = new CaregiverProfileModel();

        [BindProperty]
        public bool ChangePassword { get; set; } = false;

        public SelectList AvailabilityOptions { get; set; } = default!;

        public BusinessObjects.Caregiver CurrentCaregiver { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in and is a caregiver
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)

            {
                return RedirectToPage("/Auth/Login");
            }

            var roleIdString = HttpContext.Session.GetString("Role");
            if (roleIdString != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get account information
            var account = await _accountService.GetAccountByIdAsync((accountId ?? 0));
            if (account == null)
            {
                return NotFound();
            }

            // Get caregiver information
            CurrentCaregiver = _caregiverService.GetCaregiverByAccountId((accountId ?? 0));
            if (CurrentCaregiver == null)
            {
                return NotFound("Caregiver profile not found");
            }

            // Map account and caregiver data to the input model
            Input.AccountId = account.AccountId;
            Input.Username = account.Username;
            Input.Email = account.Email;
            Input.Phone = account.Phone;
            Input.Fullname = account.Fullname;
            Input.Address = account.Address;
            Input.Birthdate = account.Birthdate;
            Input.Hobby = account.Hobby;

            // Map caregiver-specific fields
            Input.CaregiverId = CurrentCaregiver.CaregiverId;
            Input.ExperienceYears = CurrentCaregiver.ExperienceYears;
            Input.Specialty = CurrentCaregiver.Specialty;
            Input.Certification = CurrentCaregiver.Certification;
            Input.Availability = CurrentCaregiver.Availability;

            LoadDropdownData();
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
                LoadDropdownData();
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

                // Update account details (only fields caregivers are allowed to change)
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

                // Update caregiver details
                var caregiver = _caregiverService.GetCaregiverByAccountId(account.AccountId);
                if (caregiver != null)
                {
                    caregiver.ExperienceYears = Input.ExperienceYears;
                    caregiver.Specialty = Input.Specialty;
                    caregiver.Certification = Input.Certification;
                    caregiver.Availability = Input.Availability;
                    _caregiverService.UpdateCaregiver(caregiver);
                }

                TempData["SuccessMessage"] = "Your profile was updated successfully";
                return RedirectToPage("/Caregiver/Dashboard");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating profile: {ex.Message}");
                LoadDropdownData();
                return Page();
            }
        }

        private void LoadDropdownData()
        {
            // Setup availability options for caregivers
            var availabilityOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "full-time", Text = "Full Time" },
                new SelectListItem { Value = "part-time", Text = "Part Time" },
                new SelectListItem { Value = "on-call", Text = "On Call" }
            };
            AvailabilityOptions = new SelectList(availabilityOptions, "Value", "Text");
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convert the input string to a byte array and compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }

    public class CaregiverProfileModel
    {
        public int AccountId { get; set; }

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

        // Caregiver-specific properties
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