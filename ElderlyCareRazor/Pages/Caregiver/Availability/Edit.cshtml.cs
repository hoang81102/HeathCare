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
    public class EditModel : PageModel
    {
        private readonly ICaregiverAvailabilityService _availabilityService;
        private readonly ICaregiverService _caregiverService;

        public EditModel(ICaregiverAvailabilityService availabilityService, ICaregiverService caregiverService)
        {
            _availabilityService = availabilityService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public CaregiverAvailability Availability { get; set; }

        public SelectList DaysOfWeekOptions { get; set; }
        public bool IsNewAvailability { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
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

            // Prepare day of week options
            var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
            DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");

            if (id.HasValue) // Edit existing availability
            {
                Availability = _availabilityService.GetAvailabilityById(id.Value);
                if (Availability == null)
                {
                    return NotFound("Availability not found.");
                }

                // Check if this availability belongs to the logged-in caregiver
                if (Availability.CaregiverId != caregiver.CaregiverId)
                {
                    return Forbid();
                }

                IsNewAvailability = false;
            }
            else // Create new availability
            {
                Availability = new CaregiverAvailability
                {
                    CaregiverId = caregiver.CaregiverId,
                    IsAvailable = true,
                    // Set default start and end times (8 AM to 5 PM)
                    StartTime = new TimeOnly(8, 0),
                    EndTime = new TimeOnly(17, 0)
                };
                IsNewAvailability = true;
            }

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
                // Repopulate dropdown
                var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
                DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");
                return Page();
            }

            try
            {
                // Ensure start time is before end time
                if (Availability.StartTime >= Availability.EndTime)
                {
                    ModelState.AddModelError(string.Empty, "Start time must be before end time.");
                    var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
                    DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");
                    return Page();
                }

                // If no ID is present or ID is 0, it's a new record
                if (Availability.AvailabilityId == 0)
                {
                    _availabilityService.AddAvailability(Availability);
                }
                else
                {
                    _availabilityService.UpdateAvailability(Availability);
                }

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
                ModelState.AddModelError(string.Empty, ErrorMessage);

                // Repopulate dropdown
                var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
                DaysOfWeekOptions = new SelectList(daysOfWeek, "Key", "Value");

                return Page();
            }
        }
    }
}