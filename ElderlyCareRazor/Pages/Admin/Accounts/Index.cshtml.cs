using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Admin.Accounts
{
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;

        public IndexModel(IAccountService accountService, IRoleService roleService)
        {
            _accountService = accountService;
            _roleService = roleService;
        }

        public List<Account> Accounts { get; set; } = new List<Account>();
        public Dictionary<int, string> RoleNames { get; set; } = new Dictionary<int, string>();

        public async Task<IActionResult> OnGetAsync()
        {
            //Ensure user is authorized as admin
            //if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            //{
            //    return RedirectToPage("/Auth/Login");
            //}

            Accounts = await _accountService.GetAllAccountsAsync();

            // Load role names for display
            var roles = await _roleService.GetAllRolesAsync();
            foreach (var role in roles)
            {
                RoleNames[role.RoleId] = role.RoleName;
            }

            return Page();
        }
    }
}