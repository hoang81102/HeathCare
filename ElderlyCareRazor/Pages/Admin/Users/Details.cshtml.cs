using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Admin.Users
{
    public class DetailsModel : PageModel
    {
        private readonly IAccountService _accountService;

        public DetailsModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public Account Account { get; set; } = default!;

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

            Account = account;
            return Page();
        }
    }
}
