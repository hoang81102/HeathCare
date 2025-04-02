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
    public class DetailsModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IElderService _elderService;
        private readonly IBookingService _bookingService;
        private readonly IFeedbackService _feedbackService;
        private readonly ICaregiverService _caregiverService;
        private readonly IServiceService _serviceService;

        public DetailsModel(
            IAccountService accountService,
            IElderService elderService,
            IBookingService bookingService,
            IFeedbackService feedbackService,
            ICaregiverService caregiverService,
            IServiceService serviceService)
        {
            _accountService = accountService;
            _elderService = elderService;
            _bookingService = bookingService;
            _feedbackService = feedbackService;
            _caregiverService = caregiverService;
            _serviceService = serviceService;
        }

        public Account Customer { get; set; }
        public List<Elder> Elders { get; set; }
        public List<Booking> RecentBookings { get; set; }
        public List<Feedback> Feedbacks { get; set; }
        public Dictionary<int, string> FeedbackCaregivers { get; set; } = new Dictionary<int, string>();
        public Dictionary<int, string> FeedbackServices { get; set; } = new Dictionary<int, string>();

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
                return RedirectToPage("/Admin/Customers");
            }

            // Get customer details
            Customer = await _accountService.GetAccountByIdAsync(id.Value);
            if (Customer == null)
            {
                return Page();
            }

            // Get elders associated with this customer
            Elders = _elderService.GetEldersByAccountId(id.Value);

            // Get recent bookings
            RecentBookings = _bookingService.GetBookingsByAccountId(id.Value)
                .OrderByDescending(b => b.BookingDateTime)
                .Take(5)
                .ToList();

            // Get feedback history
            try
            {
                Feedbacks = _feedbackService.GetFeedbacksByCustomerId(id.Value)
                    .OrderByDescending(f => f.FeedbackDate)
                    .ToList();

                // Get caregiver and service names for each feedback's booking
                foreach (var feedback in Feedbacks)
                {
                    try
                    {
                        var booking = _bookingService.GetBookingById(feedback.BookingId);
                        if (booking != null)
                        {
                            if (booking.CaregiverId > 0)
                            {
                                var caregiver = _caregiverService.GetCaregiverById(booking.CaregiverId);
                                if (caregiver != null && caregiver.Account != null)
                                {
                                    FeedbackCaregivers[feedback.BookingId] = caregiver.Account.Fullname;
                                }
                            }

                            if (booking.ServiceId > 0)
                            {
                                var service = _serviceService.GetServiceById(booking.ServiceId);
                                if (service != null)
                                {
                                    FeedbackServices[feedback.BookingId] = service.ServiceName;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            catch
            {
                // Handle case where no feedback exists
                Feedbacks = new List<Feedback>();
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
                    return RedirectToPage("/Admin/Customers");
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

            return RedirectToPage(new { id });
        }

    }
}