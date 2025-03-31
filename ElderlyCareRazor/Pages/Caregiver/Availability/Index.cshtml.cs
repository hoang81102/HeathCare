using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Http;

namespace ElderlyCareRazor.Pages.Caregiver.Availability
{
    public class IndexModel : PageModel
    {
        private readonly ICaregiverAvailabilityService _availabilityService;
        private readonly ICaregiverService _caregiverService;

        public IndexModel(ICaregiverAvailabilityService availabilityService, ICaregiverService caregiverService)
        {
            _availabilityService = availabilityService;
            _caregiverService = caregiverService;
        }

        public List<CaregiverAvailability> Availabilities { get; set; }
        public Dictionary<int, string> DaysOfWeek { get; set; }
        public List<string> TimeSlots { get; set; }
        public int CaregiverId { get; set; }

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

            CaregiverId = caregiver.CaregiverId;

            // Get availability data
            Availabilities = _availabilityService.GetAvailabilitiesByCaregiverId(CaregiverId);

            // Initialize day of week options
            DaysOfWeek = _availabilityService.GetDayOfWeekOptions();

            // Create time slots for the weekly overview (hourly from 6 AM to 10 PM)
            TimeSlots = new List<string>();
            for (int hour = 6; hour <= 22; hour++)
            {
                // Format with AM/PM
                string ampm = hour < 12 ? "AM" : "PM";
                int displayHour = hour <= 12 ? hour : hour - 12;
                if (displayHour == 0) displayHour = 12;

                TimeSlots.Add($"{displayHour}:00 {ampm}");
            }

            return Page();
        }

        public IActionResult OnPostToggleStatus(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }

            var availability = _availabilityService.GetAvailabilityById(id);
            if (availability == null)
            {
                return NotFound("Availability not found.");
            }

            // Toggle the availability status
            _availabilityService.ToggleAvailabilityStatus(id, !availability.IsAvailable.Value);

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }

            var availability = _availabilityService.GetAvailabilityById(id);
            if (availability == null)
            {
                return NotFound("Availability not found.");
            }

            _availabilityService.DeleteAvailability(id);

            return RedirectToPage();
        }

        public string GetDayName(int dayOfWeek)
        {
            return DaysOfWeek.ContainsKey(dayOfWeek) ? DaysOfWeek[dayOfWeek] : "Unknown";
        }

        public string GetAvailabilityClass(int dayOfWeek, string timeSlot)
        {
            if (Availabilities == null || !Availabilities.Any())
                return "";

            try
            {
                // Convert time slot string to TimeOnly
                DateTime parsedTime;
                if (!DateTime.TryParse(timeSlot, out parsedTime))
                {
                    // Try alternate format
                    parsedTime = DateTime.Parse("2000-01-01 " + timeSlot);
                }

                TimeOnly slotStart = new TimeOnly(parsedTime.Hour, parsedTime.Minute);
                TimeOnly slotEnd = slotStart.AddHours(1);

                // Check if any availability overlaps with this time slot
                bool isAvailable = Availabilities.Any(a =>
                    a.DayOfWeek == dayOfWeek &&
                    a.IsAvailable == true &&
                    ((slotStart >= a.StartTime && slotStart < a.EndTime) || // slot starts within availability
                     (slotEnd > a.StartTime && slotEnd <= a.EndTime) ||     // slot ends within availability
                     (slotStart <= a.StartTime && slotEnd >= a.EndTime)));  // slot encompasses availability

                return isAvailable ? "available-slot" : "";
            }
            catch (Exception ex)
            {
                // Log the error if needed
                Console.WriteLine($"Error in GetAvailabilityClass: {ex.Message}");
                return "";
            }
        }
    }
}