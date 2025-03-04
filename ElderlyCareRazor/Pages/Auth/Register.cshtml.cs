using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccesObjects;
using ElderlyCareRazor.Models;
using Services;
using BusinessObjects;

namespace ElderlyCareRazor.Pages.Auth
{
    public class RegisterModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(IAccountService accountService, ILogger<RegisterModel> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }

        public string errorMessage { get; set; }
        public string successMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                if (!TryValidateModel(RegisterViewModel, nameof(RegisterViewModel)) || !ModelState.IsValid)
                {
                    ModelState.AddModelError(string.Empty, "Error: Please check your input data.");
                    return Page();
                }

                if (RegisterViewModel.Password != RegisterViewModel.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return Page();
                }

                var existingUser = await _accountService.GetAccountByEmailAsync(RegisterViewModel.Email);
                var existingUsername = await _accountService.GetAccountByUsernameAsync(RegisterViewModel.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return Page();
                }

                if (existingUsername != null)
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return Page();
                }

                var newUser = new Account
                {
                    Username = RegisterViewModel.Username,
                    Address = RegisterViewModel.Address,
                    Password = RegisterViewModel.Password,
                    Fullname = RegisterViewModel.Fullname,
                    Email = RegisterViewModel.Email,
                    Phone = RegisterViewModel.Phone,
                    AccountStatus = "active",
                    RoleId = 1
                };

                await _accountService.AddAccountAsync(newUser);
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToPage("/Auth/Login");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again later.");
            }

            return Page();
        }

    }
}
