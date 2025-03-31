using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElderlyCareRazor.Pages.Caregiver
{
    public class DashboardModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly ICaregiverService _caregiverService;
        private readonly IRecordService _recordService;
        private readonly ICaregiverAvailabilityService _availabilityService;
        private readonly IFeedbackService _feedbackService;

        public DashboardModel(
            IBookingService bookingService,
            ICaregiverService caregiverService,
            IRecordService recordService,
            ICaregiverAvailabilityService availabilityService,
            IFeedbackService feedbackService)
        {
            _bookingService = bookingService;
            _caregiverService = caregiverService;
            _recordService = recordService;
            _availabilityService = availabilityService;
            _feedbackService = feedbackService;
        }

        public BusinessObjects.Caregiver CurrentCaregiver { get; set; }
        public List<Booking> UpcomingBookings { get; set; }
        public List<Booking> PendingBookings { get; set; }
        public List<Record> ActiveRecords { get; set; }
        public Dictionary<int, string> DayOfWeekMap { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, double> DetailedRatings { get; set; }
        public int TotalCompletedServices { get; set; }
        public List<Record> RecentRecords { get; set; }

        public IActionResult OnGet()
        {
            // Check if user is logged in and is a caregiver
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            
            {
                return RedirectToPage("/Auth/Login");
            }

            var roleIdString = HttpContext.Session.GetString("Role");
            if (roleIdString != "Caregiver") 
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get the caregiver information
            CurrentCaregiver = _caregiverService.GetCaregiverByAccountId((accountId ?? 0));
            if (CurrentCaregiver == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get upcoming bookings for this caregiver
            UpcomingBookings = _bookingService.GetUpcomingBookingsByCaregiverId(CurrentCaregiver.CaregiverId);

            // Get pending bookings that need action
            PendingBookings = _bookingService.GetBookingsByCaregiverId(CurrentCaregiver.CaregiverId)
                .Where(b => b.Status == "pending")
                .ToList();

            // Get active records (in-progress)
            ActiveRecords = _recordService.GetRecordsByCaregiverId(CurrentCaregiver.CaregiverId)
                .Where(r => r.Status == "InProgress")
                .ToList();

            // Get days of week mapping
            DayOfWeekMap = _availabilityService.GetDayOfWeekOptions();

            // Get caregiver rating information
            AverageRating = _feedbackService.GetAverageRatingForCaregiver(CurrentCaregiver.CaregiverId);
            DetailedRatings = _feedbackService.GetDetailedRatingsForCaregiver(CurrentCaregiver.CaregiverId);

            // Get total completed services
            TotalCompletedServices = _bookingService.GetBookingsByCaregiverId(CurrentCaregiver.CaregiverId)
                .Count(b => b.Status == "completed");

            // Get recent records (limited to last 5)
            RecentRecords = _recordService.GetRecordsByCaregiverId(CurrentCaregiver.CaregiverId)
                .OrderByDescending(r => r.LastUpdated)
                .Take(5)
                .ToList();

            return Page();
        }
    }
}