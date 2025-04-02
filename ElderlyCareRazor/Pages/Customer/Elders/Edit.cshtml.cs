using System;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Customer.Elders
{
    public class EditModel : PageModel
    {
        private readonly IElderService _elderService;

        public EditModel(IElderService elderService)
        {
            _elderService = elderService;
        }

        [BindProperty]
        public Elder Elder { get; set; }

        public IActionResult OnGet(int id)
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            Elder = _elderService.GetElderById(id);

            // Check if elder exists and belongs to the current user
            if (Elder == null || Elder.AccountId != accountId)
            {
                return Page(); // Will show the "Elder not found" message
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

            // Verify that the elder belongs to the current user
            var existingElder = _elderService.GetElderById(Elder.ElderId);
            if (existingElder == null || existingElder.AccountId != accountId)
            {
                return NotFound();
            }

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
                // Ensure the AccountId remains unchanged
                Elder.AccountId = existingElder.AccountId;

                _elderService.UpdateElder(Elder);

                TempData["SuccessMessage"] = $"Elder '{Elder.Fullname}' has been updated successfully.";
                return RedirectToPage("Details", new { id = Elder.ElderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}