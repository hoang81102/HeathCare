using BusinessObjects;
using ElderlyCareRazor.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Auth
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        private readonly IAccountService _accountService;

        public LoginModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Required(ErrorMessage = "Email or Username is required")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Data validation failed.";
                return Page();
            }

            var user = await _accountService.AuthenticateAsync(Email, Password);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Username or Password invalid.";
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.AccountId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "client" : "admin" )
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Login");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(claimsPrincipal);

            HttpContext.Session.SetString("UserId", user.AccountId.ToString());
            HttpContext.Session.SetInt32("AccountId", user.AccountId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("UserRole", user.RoleId == 1 ? "client" : user.RoleId == 2 ? "admin" : "care");
            HttpContext.Session.SetString("Role", user.RoleId == 1 ? "Admin" : user.RoleId == 2 ? "Customer" : "Caregiver");

            var userJson = JsonSerializer.Serialize(user);
            HttpContext.Session.SetString("User", userJson);

            TempData["SuccessMessage"] = "Login successful!";
            return RedirectToPage("/Index");
        }


        public IActionResult OnPostGoogleLogin()
        {
            var redirectUrl = Url.Page("/Auth/Login", "GoogleResponse", "GoogleResponse");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> OnGetGoogleResponseAsync()
        {
            if (HttpContext.Request.Query.ContainsKey("error"))
            {
                var error = HttpContext.Request.Query["error"];
                if (error == "access_denied")
                {
                    ModelState.AddModelError(string.Empty, "You have canceled login. Please try again.");
                    return RedirectToPage("/Auth/Login");
                }
            }

            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
            {
                return RedirectToPage("/Auth/Login");
            }

            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var userName = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var fullName = authenticateResult.Principal.FindFirst(ClaimTypes.Name)?.Value;
            var phoneNumber = authenticateResult.Principal.FindFirst("phone_number")?.Value;

            string emailUsername = "";
            string emailDomain = "";

            if (!string.IsNullOrEmpty(email))
            {
                var emailParts = email.Split('@');
                if (emailParts.Length == 2)
                {
                    emailUsername = emailParts[0];
                    emailDomain = emailParts[1];
                }
            }

            var account = await _accountService.GetAccountByEmailAsync(email);

            if (account == null)
            {
                var userNameIsValid = await _accountService.GetAccountByUsernameAsync(emailUsername);
                var userNameSignup = emailUsername;
                var random = new Random();

                while (userNameIsValid != null)
                {
                    userNameSignup = emailUsername + random.Next(1000, 9999).ToString();
                    userNameIsValid = await _accountService.GetAccountByUsernameAsync(userNameSignup);
                }

                string GenerateRandomPassword(int length = 8)
                {
                    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                    return new string(Enumerable.Repeat(chars, length)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                }

                var randomPassword = GenerateRandomPassword();

                var newAccount = new Account
                {
                    Username = userNameSignup,
                    Email = email,
                    Password = randomPassword,
                    Fullname = fullName,
                    Phone = phoneNumber,
                    RoleId = 1,
                    AccountStatus = "Active"
                };

                await _accountService.AddAccountAsync(newAccount);

                account = newAccount;
                HttpContext.Session.SetString("UserId", account.AccountId.ToString());
                HttpContext.Session.SetInt32("AccountId", account.AccountId);

                HttpContext.Session.SetString("Username", account.Username);
                HttpContext.Session.SetString("UserRole", account.RoleId == 1 ? "client" : account.RoleId == 2 ? "admin" : "care");
                HttpContext.Session.SetString("Role", account.RoleId == 1 ? "Admin" : account.RoleId == 2 ? "Customer" : "Caregiver");

            }
            else
            {
                if (account.AccountStatus == "Disabled")
                {
                    ModelState.AddModelError(string.Empty, "This account has been locked.");
                    return Page();
                }

                HttpContext.Session.SetString("UserId", account.AccountId.ToString());
                HttpContext.Session.SetInt32("AccountId", account.AccountId);

                HttpContext.Session.SetString("Username", account.Username);
                HttpContext.Session.SetString("UserRole", account.RoleId == 1 ? "client" : "admin");
                HttpContext.Session.SetString("Role", account.RoleId == 1 ? "Admin" : account.RoleId == 2 ? "Customer" : "Caregiver");

            }

            return RedirectToPage("/Index");
        }

    }
}
