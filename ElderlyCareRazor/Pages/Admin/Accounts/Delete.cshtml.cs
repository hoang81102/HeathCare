using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Admin.Accounts
{
    public class DeleteModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly ICaregiverService _caregiverService;

        public DeleteModel(IAccountService accountService, IRoleService roleService, ICaregiverService caregiverService)
        {
            _accountService = accountService;
            _roleService = roleService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public Account Account { get; set; } = default!;

        public Role Role { get; set; } = default!;

        public BusinessObjects.Caregiver? Caregiver { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Ensure user is authorized as admin
            //if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            //{
            //    return RedirectToPage("/Auth/Login");
            //}

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

            // Load role information
            var role = await _roleService.GetRoleByIdAsync(account.RoleId);
            if (role != null)
            {
                Role = role;
            }

            // If account is a caregiver, load caregiver details
            if (account.RoleId == 3) // Assuming 3 is for Caregiver
            {
                Caregiver = _caregiverService.GetCaregiverByAccountId(account.AccountId);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _accountService.GetAccountByIdAsync(id.Value);
            if (account == null)
            {
                return NotFound();
            }

            try
            {
                // Delete the account
                await _accountService.DeleteAccountAsync(id.Value);
                TempData["SuccessMessage"] = "Account deleted successfully";
                return RedirectToPage("./Index");
            }
            catch (System.Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting account: {ex.Message}";
                return RedirectToPage("./Delete", new { id });
            }
        }
    }
}