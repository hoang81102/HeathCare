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

namespace ElderlyCareRazor.Pages.Admin.Accounts
{
    public class CreateModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly ICaregiverService _caregiverService;

        public CreateModel(IAccountService accountService, IRoleService roleService, ICaregiverService caregiverService)
        {
            _accountService = accountService;
            _roleService = roleService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public AccountInputModel Input { get; set; } = new AccountInputModel();

        [BindProperty]
        public CaregiverInputModel CaregiverInput { get; set; } = new CaregiverInputModel();

        public SelectList RoleList { get; set; } = default!;
        public SelectList AvailabilityOptions { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDropdownDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
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

            if (!ModelState.IsValid)
            {
                await LoadDropdownDataAsync();
                return Page();
            }

            try
            {
                // Hash the password
                string hashedPassword = HashPassword(Input.Password);

                // Create the account
                var account = new Account
                {
                    Username = Input.Username,
                    Password = hashedPassword,
                    Email = Input.Email,
                    Phone = Input.Phone,
                    Fullname = Input.Fullname,
                    Address = Input.Address,
                    Birthdate = Input.Birthdate,
                    Hobby = Input.Hobby,
                    AccountStatus = "active",
                    RoleId = Input.RoleId
                };

                await _accountService.AddAccountAsync(account);

                // If the role is Caregiver, add caregiver details
                if (Input.RoleId == 3) // Assuming 3 is for Caregiver
                {
                    // Get the newly created account id
                    var createdAccount = await _accountService.GetAccountByUsernameAsync(Input.Username);
                    if (createdAccount != null)
                    {
                        var caregiver = new BusinessObjects.Caregiver
                        {
                            AccountId = createdAccount.AccountId,
                            ExperienceYears = CaregiverInput.ExperienceYears,
                            Specialty = CaregiverInput.Specialty,
                            Certification = CaregiverInput.Certification,
                            Availability = CaregiverInput.Availability
                        };

                        _caregiverService.AddCaregiver(caregiver);
                    }
                }

                TempData["SuccessMessage"] = "Account created successfully";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error creating account: {ex.Message}");
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

    public class AccountInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

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
        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }

    public class CaregiverInputModel
    {
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