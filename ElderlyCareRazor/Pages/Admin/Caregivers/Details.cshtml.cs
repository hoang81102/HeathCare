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
    public class DetailsModel : PageModel
    {
        private readonly ICaregiverService _caregiverService;
        private readonly IBookingService _bookingService;
        private readonly IFeedbackService _feedbackService;
        private readonly ICaregiverAvailabilityService _availabilityService;
        private readonly IAccountService _accountService;

        public DetailsModel(
            ICaregiverService caregiverService,
            IBookingService bookingService,
            IFeedbackService feedbackService,
            ICaregiverAvailabilityService availabilityService,
            IAccountService accountService)
        {
            _caregiverService = caregiverService;
            _bookingService = bookingService;
            _feedbackService = feedbackService;
            _availabilityService = availabilityService;
            _accountService = accountService;
        }

        public BusinessObjects.Caregiver Caregiver { get; set; }
        public List<Booking> RecentBookings { get; set; }
        public List<Feedback> RecentFeedbacks { get; set; }
        public List<CaregiverAvailability> Availabilities { get; set; }
        public double AverageRating { get; set; }
        public Dictionary<string, double> DetailedRatings { get; set; } = new Dictionary<string, double>
        {
            { "caregiverProfessionalism", 0 },
            { "serviceQuality", 0 },
            { "overallExperience", 0 }
        };

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // Check if user is logged in as admin
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Admin")
            {
                return RedirectToPage("/Auth/Login");
            }

            if (id == null)
            {
                return RedirectToPage("/Admin/Caregivers");
            }

            // Get caregiver details
            Caregiver = _caregiverService.GetCaregiverById(id.Value);
            if (Caregiver == null)
            {
                return Page();
            }

            // Get recent bookings
            RecentBookings = _bookingService.GetBookingsByCaregiverId(id.Value)
                .OrderByDescending(b => b.BookingDateTime)
                .Take(5)
                .ToList();

            // Get availability schedule
            Availabilities = _availabilityService.GetAvailabilitiesByCaregiverId(id.Value);

            // Get feedback and ratings
            try
            {
                AverageRating = _feedbackService.GetAverageRatingForCaregiver(id.Value);
                DetailedRatings = _feedbackService.GetDetailedRatingsForCaregiver(id.Value);

                // Get recent feedbacks
                RecentFeedbacks = _feedbackService.GetFeedbacksByCaregiverId(id.Value)
                    .OrderByDescending(f => f.FeedbackDate)
                    .Take(5)
                    .ToList();
            }
            catch
            {
                // Handle case where no feedback exists
                AverageRating = 0;
                RecentFeedbacks = new List<Feedback>();
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
                    return RedirectToPage("/Admin/Caregivers");
                }

                // Get the associated account
                var account = await _accountService.GetAccountByIdAsync(caregiver.AccountId);
                if (account == null)
                {
                    TempData["ErrorMessage"] = "Account not found.";
                    return RedirectToPage("/Admin/Caregivers");
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

            return RedirectToPage(new { id });
        }
    }
}