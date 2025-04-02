using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Http;

namespace ElderlyCareRazor.Pages.Caregiver.Availability
{
    public class CreateModel : PageModel
    {
        private readonly ICaregiverAvailabilityService _availabilityService;
        private readonly ICaregiverService _caregiverService;

        public CreateModel(ICaregiverAvailabilityService availabilityService, ICaregiverService caregiverService)
        {
            _availabilityService = availabilityService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public CaregiverAvailability Availability { get; set; }

        public SelectList DaysOfWeekOptions { get; set; }
        public List<CaregiverAvailability> ExistingAvailabilities { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is logged in and has caregiver role
            if (HttpContext.Session.GetString("Role") != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }

            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (!accountId.HasValue)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get caregiver information
            var caregiver = _caregiverService.GetCaregiverByAccountId(accountId.Value);
            if (caregiver == null)
            {
                return NotFound("Caregiver profile not found.");
            }

            // Initialize new availability
            Availability = new CaregiverAvailability
            {
                CaregiverId = caregiver.CaregiverId,
                IsAvailable = true,
                // Set default start and end times (8 AM to 5 PM)
                StartTime = new TimeOnly(8, 0),
                EndTime = new TimeOnly(17, 0)
            };

            // Prepare day of week options
            var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
            DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");

            // Get existing availabilities for sidebar display
            ExistingAvailabilities = _availabilityService.GetAvailabilitiesByCaregiverId(caregiver.CaregiverId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Check if user is logged in and has caregiver role
            if (HttpContext.Session.GetString("Role") != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }
            ModelState.Remove("Availability.Caregiver");
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns and existing availabilities
                var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
                DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");

                var accountId = HttpContext.Session.GetInt32("AccountId");
                var caregiver = _caregiverService.GetCaregiverByAccountId(accountId.Value);
                ExistingAvailabilities = _availabilityService.GetAvailabilitiesByCaregiverId(caregiver.CaregiverId);

                return Page();
            }

            try
            {
                // Ensure start time is before end time
                if (Availability.StartTime >= Availability.EndTime)
                {
                    ModelState.AddModelError(string.Empty, "Start time must be before end time.");

                    // Repopulate dropdowns and existing availabilities
                    var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
                    DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");

                    var accountId = HttpContext.Session.GetInt32("AccountId");
                    var caregiver = _caregiverService.GetCaregiverByAccountId(accountId.Value);
                    ExistingAvailabilities = _availabilityService.GetAvailabilitiesByCaregiverId(caregiver.CaregiverId);

                    return Page();
                }

                // Add the new availability
                _availabilityService.AddAvailability(Availability);

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                ModelState.AddModelError(string.Empty, ErrorMessage);

                // Repopulate dropdowns and existing availabilities
                var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
                DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");

                var accountId = HttpContext.Session.GetInt32("AccountId");
                var caregiver = _caregiverService.GetCaregiverByAccountId(accountId.Value);
                ExistingAvailabilities = _availabilityService.GetAvailabilitiesByCaregiverId(caregiver.CaregiverId);

                return Page();
            }
        }

        public string GetDayName(int dayOfWeek)
        {
            var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
            return daysOfWeek.ContainsKey(dayOfWeek) ? daysOfWeek[dayOfWeek] : "Unknown";
        }
    }
}