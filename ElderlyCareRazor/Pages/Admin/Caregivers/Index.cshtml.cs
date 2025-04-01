using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Http;

namespace ElderlyCareRazor.Pages.Admin.Caregivers
{
    public class IndexModel : PageModel
    {
        private readonly ICaregiverService _caregiverService;
        private readonly IFeedbackService _feedbackService;
        private readonly IAccountService _accountService;

        public IndexModel(
            ICaregiverService caregiverService,
            IFeedbackService feedbackService,
            IAccountService accountService)
        {
            _caregiverService = caregiverService;
            _feedbackService = feedbackService;
            _accountService = accountService;
        }

        public List<BusinessObjects.Caregiver> Caregivers { get; set; }
        public Dictionary<int, double> CaregiverRatings { get; set; } = new Dictionary<int, double>();

        [BindProperty(SupportsGet = true)]
        public string SpecialtyFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? ExperienceFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string AvailabilityFilter { get; set; }

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

            // Get all caregivers
            Caregivers = _caregiverService.GetAllCaregivers();

            // Apply filters if provided
            if (!string.IsNullOrEmpty(SpecialtyFilter))
            {
                Caregivers = Caregivers.Where(c =>
                    c.Specialty != null &&
                    c.Specialty.Contains(SpecialtyFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (ExperienceFilter.HasValue)
            {
                Caregivers = Caregivers.Where(c => c.ExperienceYears >= ExperienceFilter.Value).ToList();
            }

            if (!string.IsNullOrEmpty(AvailabilityFilter))
            {
                Caregivers = Caregivers.Where(c => c.Availability == AvailabilityFilter).ToList();
            }

            if (!string.IsNullOrEmpty(StatusFilter))
            {
                Caregivers = Caregivers.Where(c => c.Account.AccountStatus == StatusFilter).ToList();
            }

            // Get ratings for all caregivers
            foreach (var caregiver in Caregivers)
            {
                try
                {
                    double rating = _feedbackService.GetAverageRatingForCaregiver(caregiver.CaregiverId);
                    if (rating > 0)
                    {
                        CaregiverRatings[caregiver.CaregiverId] = rating;
                    }
                }
                catch
                {
                    // Handle any errors getting ratings
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
                // Get the caregiver to update
                var caregiver = _caregiverService.GetCaregiverById(id);
                if (caregiver == null)
                {
                    TempData["ErrorMessage"] = "Caregiver not found.";
                    return RedirectToPage();
                }

                // Get the associated account
                var account = await _accountService.GetAccountByIdAsync(caregiver.AccountId);
                if (account == null)
                {
                    TempData["ErrorMessage"] = "Account not found.";
                    return RedirectToPage();
                }

                // Update account status
                account.AccountStatus = status;
                await _accountService.UpdateAccountAsync(account);

                TempData["SuccessMessage"] = $"Caregiver status updated successfully to {status}.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating caregiver status: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}