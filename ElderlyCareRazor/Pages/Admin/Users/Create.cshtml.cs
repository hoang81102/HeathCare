using BusinessObjects;
using ElderlyCareRazor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;

namespace ElderlyCareRazor.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;

        public CreateModel(IAccountService accountService, IRoleService roleService)
        {
            _accountService = accountService;
            _roleService = roleService;
        }

        [BindProperty]
        public AccountViewModel AccountViewModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "admin")
            {
                TempData["ErrorMessage"] = "You do not have permission to access this page.";
                return RedirectToPage("/Auth/Login");
            }

            var roles = await _roleService.GetAllRolesAsync();
            ViewData["RoleId"] = new SelectList(roles, "RoleId", "RoleName");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "admin")
            {
                TempData["ErrorMessage"] = "You do not have permission to perform this action.";
                return RedirectToPage("/Auth/Login");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            var existingUser = await _accountService.GetAccountByEmailAsync(AccountViewModel.Email);
            var existingUsername = await _accountService.GetAccountByUsernameAsync(AccountViewModel.Username);

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

            var newAccount = new Account
            {
                Username = AccountViewModel.Username,
                Password = AccountViewModel.Password,
                Phone = AccountViewModel.Phone,
                Email = AccountViewModel.Email,
                Fullname = AccountViewModel.Fullname,
                Address = AccountViewModel.Address,
                Birthdate = AccountViewModel.Birthdate,
                Hobby = AccountViewModel.Hobby,
                AccountStatus = AccountViewModel.AccountStatus,
                RoleId = AccountViewModel.RoleId
            };

            await _accountService.AddAccountAsync(newAccount);
            return RedirectToPage("./Index");
        }
    }
}
