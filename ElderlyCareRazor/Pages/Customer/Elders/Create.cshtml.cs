using System;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Customer.Elders
{
    public class CreateModel : PageModel
    {
        private readonly IElderService _elderService;

        public CreateModel(IElderService elderService)
        {
            _elderService = elderService;
        }

        [BindProperty]
        public Elder Elder { get; set; } = new Elder();

        public IActionResult OnGet()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Set the account ID for the elder
            Elder.AccountId = accountId.Value;

            // Perform validation
            if (string.IsNullOrWhiteSpace(Elder.Fullname))
            {
                ModelState.AddModelError("Elder.Fullname", "Full name is required.");
            }
            ModelState.Remove("Elder.Account");
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _elderService.AddElder(Elder);

                TempData["SuccessMessage"] = $"Elder '{Elder.Fullname}' has been added successfully.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}