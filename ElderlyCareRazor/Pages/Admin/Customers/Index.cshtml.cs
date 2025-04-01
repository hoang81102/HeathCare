using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Http;

namespace ElderlyCareRazor.Pages.Admin.Customers
{
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IElderService _elderService;
        private readonly IBookingService _bookingService;
        private readonly IRoleService _roleService;

        public IndexModel(
            IAccountService accountService,
            IElderService elderService,
            IBookingService bookingService,
            IRoleService roleService)
        {
            _accountService = accountService;
            _elderService = elderService;
            _bookingService = bookingService;
            _roleService = roleService;
        }

        public List<Account> Customers { get; set; }
        public Dictionary<int, int> CustomerElders { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> CustomerBookings { get; set; } = new Dictionary<int, int>();

        [BindProperty(SupportsGet = true)]
        public string NameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string EmailFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string PhoneFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in as admin
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Admin")
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get all customers (accounts with roleId 2)
            var allAccounts = await _accountService.GetAllAccountsAsync();
            var customerRoleId = (await _roleService.GetAllRolesAsync())
                .FirstOrDefault(r => r.RoleName.Equals("Customer", StringComparison.OrdinalIgnoreCase))?.RoleId ?? 2;

            Customers = allAccounts
                .Where(a => a.RoleId == customerRoleId)
                .ToList();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(NameFilter))
            {
                Customers = Customers
                    .Where(c => c.Fullname != null &&
                           c.Fullname.Contains(NameFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(EmailFilter))
            {
                Customers = Customers
                    .Where(c => c.Email != null &&
                           c.Email.Contains(EmailFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(PhoneFilter))
            {
                Customers = Customers
                    .Where(c => c.Phone != null &&
                           c.Phone.Contains(PhoneFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                Customers = Customers
                    .Where(c => c.AccountStatus == StatusFilter)
                    .ToList();
            }

            // Get elder count for each customer
            foreach (var customer in Customers)
            {
                var elders = _elderService.GetEldersByAccountId(customer.AccountId);
                if (elders != null && elders.Any())
                {
                    CustomerElders[customer.AccountId] = elders.Count;
                }

                var bookings = _bookingService.GetBookingsByAccountId(customer.AccountId);
                if (bookings != null && bookings.Any())
                {
                    CustomerBookings[customer.AccountId] = bookings.Count;
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnGetToggleStatusAsync(int id, string status)
        {
            // Check if user is logged in as admin
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Admin")
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                // Get the account to update
                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                {
                    TempData["ErrorMessage"] = "Customer account not found.";
                    return RedirectToPage();
                }

                // Update account status
                account.AccountStatus = status;
                await _accountService.UpdateAccountAsync(account);

                TempData["SuccessMessage"] = $"Customer account status updated successfully to {status}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating customer account status: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}