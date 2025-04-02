using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Admin.Users
{
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;

        public IList<Account> Accounts { get; set; } = default!;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || userRole != "admin")
            {
                TempData["ErrorMessage"] = "You do not have permission to access this page.";
                return RedirectToPage("/Auth/Login");
            }

            Accounts = await _accountService.GetAllAccountsAsync();
            return Page();
        }
    }
}
