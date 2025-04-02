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
    public class ConflictsModel : PageModel
    {
        private readonly ICaregiverAvailabilityService _availabilityService;
        private readonly ICaregiverService _caregiverService;
        private readonly IBookingService _bookingService;

        public ConflictsModel(
            ICaregiverAvailabilityService availabilityService,
            ICaregiverService caregiverService,
            IBookingService bookingService)
        {
            _availabilityService = availabilityService;
            _caregiverService = caregiverService;
            _bookingService = bookingService;
        }

        public Dictionary<int, List<CaregiverAvailability>> OverlappingSlots { get; set; }
        public List<Booking> UpcomingBookings { get; set; }
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

            // Get all availability records for this caregiver
            var availabilities = _availabilityService.GetAvailabilitiesByCaregiverId(CaregiverId);

            // Find overlapping time slots
            OverlappingSlots = new Dictionary<int, List<CaregiverAvailability>>();

            // Group by day of week
            var groupedByDay = availabilities.GroupBy(a => a.DayOfWeek).ToDictionary(g => g.Key, g => g.ToList());

            // For each day, check for overlaps
            foreach (var day in groupedByDay)
            {
                var daySlots = day.Value;

                // Check each pair of slots for overlap
                var overlaps = new List<CaregiverAvailability>();

                for (int i = 0; i < daySlots.Count; i++)
                {
                    for (int j = i + 1; j < daySlots.Count; j++)
                    {
                        // Check if these two slots overlap
                        if ((daySlots[i].StartTime <= daySlots[j].StartTime && daySlots[i].EndTime > daySlots[j].StartTime) ||
                            (daySlots[i].StartTime < daySlots[j].EndTime && daySlots[i].EndTime >= daySlots[j].EndTime) ||
                            (daySlots[i].StartTime >= daySlots[j].StartTime && daySlots[i].EndTime <= daySlots[j].EndTime))
                        {
                            // If we find an overlap, add both slots to the list if they're not already there
                            if (!overlaps.Contains(daySlots[i]))
                                overlaps.Add(daySlots[i]);

                            if (!overlaps.Contains(daySlots[j]))
                                overlaps.Add(daySlots[j]);
                        }
                    }
                }

                // If we found any overlaps for this day, add them to the result
                if (overlaps.Any())
                {
                    OverlappingSlots.Add(day.Key, overlaps);
                }
            }

            // Get upcoming bookings for this caregiver
            UpcomingBookings = _bookingService.GetUpcomingBookingsByCaregiverId(CaregiverId);

            return Page();
        }

        public string GetDayName(int dayOfWeek)
        {
            var daysOfWeek = _availabilityService.GetDayOfWeekOptions();
            return daysOfWeek.ContainsKey(dayOfWeek) ? daysOfWeek[dayOfWeek] : "Unknown";
        }

        public string GetStatusBadgeClass(string status)
        {
            return status?.ToLower() switch
            {
                "pending" => "bg-warning",
                "accepted" => "bg-success",
                "rejected" => "bg-danger",
                "in-progress" => "bg-info",
                "completed" => "bg-secondary",
                "canceled" => "bg-dark",
                _ => "bg-secondary"
            };
        }
    }
}