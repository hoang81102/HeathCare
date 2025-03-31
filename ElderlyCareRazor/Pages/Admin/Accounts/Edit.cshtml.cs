using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Admin.Accounts
{
    public class EditModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly ICaregiverService _caregiverService;

        public EditModel(IAccountService accountService, IRoleService roleService, ICaregiverService caregiverService)
        {
            _accountService = accountService;
            _roleService = roleService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public AccountEditModel Input { get; set; } = new AccountEditModel();

        [BindProperty]
        public CaregiverEditModel CaregiverInput { get; set; } = new CaregiverEditModel();

        [BindProperty]
        public bool ChangePassword { get; set; } = false;

        public SelectList RoleList { get; set; } = default!;
        public SelectList AvailabilityOptions { get; set; } = default!;
        public SelectList StatusOptions { get; set; } = default!;

        private bool IsCaregiverRole { get; set; } = false;
        private BusinessObjects.Caregiver? ExistingCaregiver { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Ensure user is authorized as admin
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)

            {
                return RedirectToPage("/Auth/Login");
            }

            var roleIdString = HttpContext.Session.GetString("Role");
            if (roleIdString != "Admin")
            {
                return RedirectToPage("/Auth/Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            var account = await _accountService.GetAccountByIdAsync(id.Value);
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

            // Check if account is a caregiver
            IsCaregiverRole = account.RoleId == 3; // Assuming 3 is for Caregiver
            if (IsCaregiverRole)
            {
                ExistingCaregiver = _caregiverService.GetCaregiverByAccountId(account.AccountId);
                if (ExistingCaregiver != null)
                {
                    CaregiverInput.CaregiverId = ExistingCaregiver.CaregiverId;
                    CaregiverInput.ExperienceYears = ExistingCaregiver.ExperienceYears;
                    CaregiverInput.Specialty = ExistingCaregiver.Specialty;
                    CaregiverInput.Certification = ExistingCaregiver.Certification;
                    CaregiverInput.Availability = ExistingCaregiver.Availability;
                }
            }

            await LoadDropdownDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Fix ModelState validation for non-caregiver accounts
            if (Input.RoleId != 3) // If not a caregiver role
            {
                // Remove validation errors for caregiver properties
                foreach (var key in ModelState.Keys.ToList())
                {
                    if (key.StartsWith("CaregiverInput."))
                    {
                        ModelState.Remove(key);
                    }
                }
            }

            // Handle password validation only if changing password is checked
            if (!ChangePassword)
            {
                ModelState.Remove("Input.NewPassword");
                ModelState.Remove("Input.ConfirmNewPassword");
            }

            if (!ModelState.IsValid)
            {
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
                account.Username = Input.Username;
                account.Email = Input.Email;
                account.Phone = Input.Phone;
                account.Fullname = Input.Fullname;
                account.Address = Input.Address;
                account.Birthdate = Input.Birthdate;
                account.Hobby = Input.Hobby;
                account.AccountStatus = Input.AccountStatus;
                account.RoleId = Input.RoleId;

                // Set password to empty string by default to keep existing password
                account.Password = string.Empty;

                // If changing password and a new password was provided
                if (ChangePassword && !string.IsNullOrWhiteSpace(Input.NewPassword))
                {
                    // No need to hash here as it will be handled by UpdateAccountAsync
                    account.Password = Input.NewPassword;
                }

                await _accountService.UpdateAccountAsync(account);

                // Handle caregiver details
                bool wasCaregiver = _caregiverService.GetCaregiverByAccountId(account.AccountId) != null;
                bool isNowCaregiver = Input.RoleId == 3; // Assuming 3 is for Caregiver

                if (isNowCaregiver)
                {
                    // Only validate and process caregiver input if the role is caregiver
                    if (wasCaregiver)
                    {
                        // Update existing caregiver
                        var caregiver = _caregiverService.GetCaregiverByAccountId(account.AccountId);
                        if (caregiver != null)
                        {
                            caregiver.ExperienceYears = CaregiverInput.ExperienceYears;
                            caregiver.Specialty = CaregiverInput.Specialty;
                            caregiver.Certification = CaregiverInput.Certification;
                            caregiver.Availability = CaregiverInput.Availability;
                            _caregiverService.UpdateCaregiver(caregiver);
                        }
                    }
                    else
                    {
                        // Create new caregiver
                        var caregiver = new BusinessObjects.Caregiver
                        {
                            AccountId = account.AccountId,
                            ExperienceYears = CaregiverInput.ExperienceYears,
                            Specialty = CaregiverInput.Specialty,
                            Certification = CaregiverInput.Certification,
                            Availability = CaregiverInput.Availability
                        };
                        _caregiverService.AddCaregiver(caregiver);
                    }
                }
                // Note: We don't delete caregiver records if role changes from caregiver to something else
                // as it might be useful to retain the information

                TempData["SuccessMessage"] = "Account updated successfully";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error updating account: {ex.Message}");
                await LoadDropdownDataAsync();
                return Page();
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            // Load roles for dropdown
            var roles = await _roleService.GetAllRolesAsync();
            RoleList = new SelectList(roles, "RoleId", "RoleName");

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