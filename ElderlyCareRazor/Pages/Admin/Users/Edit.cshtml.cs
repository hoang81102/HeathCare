using BusinessObjects;
using ElderlyCareRazor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Admin.Users
{
    public class EditModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;

        public EditModel(IAccountService accountService, IRoleService roleService)
        {
            _accountService = accountService;
            _roleService = roleService;
        }

        [BindProperty]
        public AccountViewModel AccountViewModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Check if user is an admin
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole == null || userRole != "admin")
            {
                TempData["ErrorMessage"] = "You do not have permission to access this page.";
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

            AccountViewModel = new AccountViewModel
            {
                AccountId = account.AccountId,
                Username = account.Username,
                Password = account.Password,
                Phone = account.Phone,
                Email = account.Email,
                Fullname = account.Fullname,
                Address = account.Address,
                Birthdate = account.Birthdate,
                Hobby = account.Hobby,
                AccountStatus = account.AccountStatus,
                RoleId = account.RoleId
            }; ;
            ViewData["RoleId"] = new SelectList(await _roleService.GetAllRolesAsync(), "RoleId", "RoleName");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var account = await _accountService.GetAccountByIdAsync(AccountViewModel.AccountId);
            if (account == null)
            {
                return NotFound();
            }

            var newAccount = new Account
            {
                AccountId = AccountViewModel.AccountId, 
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
            await _accountService.UpdateAccountAsync(newAccount);

            TempData["SuccessMessage"] = "Account updated successfully!";
            return RedirectToPage("./Index");
        }

    }
}
