using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Http;

namespace ElderlyCareRazor.Pages.Customer.Bookings
{
    public class CompletedBookingDetailsModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IElderService _elderService;
        private readonly ICaregiverService _caregiverService;
        private readonly IAccountService _accountService;
        private readonly IRecordService _recordService;
        private readonly IFeedbackService _feedbackService;

        public CompletedBookingDetailsModel(
            IBookingService bookingService,
            IServiceService serviceService,
            IServiceCategoryService serviceCategoryService,
            IElderService elderService,
            ICaregiverService caregiverService,
            IAccountService accountService,
            IRecordService recordService,
            IFeedbackService feedbackService)
        {
            _bookingService = bookingService;
            _serviceService = serviceService;
            _serviceCategoryService = serviceCategoryService;
            _elderService = elderService;
            _caregiverService = caregiverService;
            _accountService = accountService;
            _recordService = recordService;
            _feedbackService = feedbackService;
        }

        public Booking Booking { get; set; }
        public Service Service { get; set; }
        public ServiceCategory ServiceCategory { get; set; }
        public Elder Elder { get; set; }
        public string CaregiverName { get; set; } = "Unknown";
        public Record Record { get; set; }
        public BusinessObjects.Feedback Feedback { get; set; }
        public bool HasFeedback { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // Check if user is logged in as a customer
                int? accountId = HttpContext.Session.GetInt32("AccountId");
                string role = HttpContext.Session.GetString("Role");

                if (accountId == null || role != "Customer")
                {
                    return RedirectToPage("/Auth/Login");
                }

                // Get the booking with minimal error risk
                try
                {
                    Booking = _bookingService.GetBookingById(id);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error loading booking: {ex.Message}");
                    return Page();
                }

                // Security check: ensure the booking belongs to the logged-in customer
                if (Booking == null || Booking.AccountId != accountId.Value)
                {
                    ModelState.AddModelError("", "Booking not found or you don't have permission to view it.");
                    return Page();
                }

                // Load related data with error handling for each step
                try
                {
                    Service = _serviceService.GetServiceById(Booking.ServiceId);

                    if (Service != null)
                    {
                        ServiceCategory = _serviceCategoryService.GetCategoryById(Service.CategoryId);
                    }

                    if (Booking.ElderId.HasValue)
                    {
                        Elder = _elderService.GetElderById(Booking.ElderId.Value);
                    }

                    // Get caregiver information
                    var caregiver = _caregiverService.GetCaregiverById(Booking.CaregiverId);
                    if (caregiver != null)
                    {
                        var caregiverAccount = await _accountService.GetAccountByIdAsync(caregiver.AccountId);
                        if (caregiverAccount != null)
                        {
                            CaregiverName = caregiverAccount.Fullname;
                        }
                    }

                    // Get the record
                    Record = _recordService.GetRecordByBookingId(id);

                    // Check if feedback exists
                    var feedbacks = _feedbackService.GetFeedbacksByBookingId(id);
                    HasFeedback = (feedbacks != null && feedbacks.Any());
                    if (HasFeedback)
                    {
                        Feedback = feedbacks.FirstOrDefault();
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error loading related data: {ex.Message}");
                    // Continue anyway since we have the basic booking information
                }

                return Page();
            }
            catch (Exception ex)
            {
                // Catch-all for any unexpected errors
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return Page();
            }
        }
    }
}