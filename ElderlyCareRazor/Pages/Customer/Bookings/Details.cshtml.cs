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
    public class DetailsModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingTimeSlotService _bookingTimeSlotService;
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IElderService _elderService;
        private readonly ICaregiverService _caregiverService;
        private readonly IRecordService _recordService;
        private readonly IFeedbackService _feedbackService;

        public DetailsModel(
            IBookingService bookingService,
            IBookingTimeSlotService bookingTimeSlotService,
            IServiceService serviceService,
            IServiceCategoryService serviceCategoryService,
            IElderService elderService,
            ICaregiverService caregiverService,
            IRecordService recordService,
            IFeedbackService feedbackService)
        {
            _bookingService = bookingService;
            _bookingTimeSlotService = bookingTimeSlotService;
            _serviceService = serviceService;
            _serviceCategoryService = serviceCategoryService;
            _elderService = elderService;
            _caregiverService = caregiverService;
            _recordService = recordService;
            _feedbackService = feedbackService;
        }

        public Booking Booking { get; set; }
        public BookingTimeSlot BookingTimeSlot { get; set; }
        public Service Service { get; set; }
        public ServiceCategory ServiceCategory { get; set; }
        public Elder Elder { get; set; }
        public BusinessObjects.Caregiver Caregiver { get; set; }
        public Record Record { get; set; }
        public BusinessObjects.Feedback Feedback { get; set; }
        public bool CanBeCancelled { get; set; }

        public IActionResult OnGet(int id)
        {
            // Check if user is logged in
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)

            {
                return RedirectToPage("/Auth/Login");
            }

            var roleIdString = HttpContext.Session.GetString("Role");
            if (roleIdString != "Customer")
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get booking details
            Booking = _bookingService.GetBookingById(id);

            // Check if booking exists and belongs to the current user
            if (Booking == null || Booking.AccountId != accountId)
            {
                return Page();
            }

            // Load related data
            LoadRelatedData();

            // Determine if booking can be cancelled
            CanBeCancelled = CanCancel();

            return Page();
        }

        private void LoadRelatedData()
        {
            // Load service information
            Service = _serviceService.GetServiceById(Booking.ServiceId);
            if (Service != null)
            {
                ServiceCategory = _serviceCategoryService.GetCategoryById(Service.CategoryId);
            }

            // Load elder information
            Elder = _elderService.GetElderById(Booking.ElderId ?? 0);

            // Load caregiver information
            Caregiver = _caregiverService.GetCaregiverById(Booking.CaregiverId);

            // Load booking time slot
            var timeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(Booking.BookingId);
            BookingTimeSlot = timeSlots.FirstOrDefault();

            // Load record if exists
            Record = _recordService.GetRecordByBookingId(Booking.BookingId);

            // Load feedback if exists
            var feedbacks = _feedbackService.GetFeedbacksByBookingId(Booking.BookingId);
            Feedback = feedbacks.FirstOrDefault();
        }

        private bool CanCancel()
        {
            // A booking can be cancelled if:
            // 1. It's in pending or accepted status
            // 2. It's not in the past
            // 3. It's not already in progress or completed or cancelled

            if (Booking.Status != "pending" && Booking.Status != "accepted")
            {
                return false;
            }

            if (BookingTimeSlot != null)
            {
                // If the booking date is in the past, it can't be cancelled
                if (BookingTimeSlot.BookingDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return false;
                }

                // If the booking is today, check if the start time has passed
                if (BookingTimeSlot.BookingDate == DateOnly.FromDateTime(DateTime.Today) &&
                    BookingTimeSlot.StartTime < TimeOnly.FromDateTime(DateTime.Now))
                {
                    return false;
                }
            }

            return true;
        }

        public IActionResult OnPostCancelBooking(int id)
        {
            // Check if user is logged in
            var accountIdString = HttpContext.Session.GetInt32("AccountId");
            if (accountIdString == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get booking details
            var booking = _bookingService.GetBookingById(id);

            // Check if booking exists and belongs to the current user
            if (booking == null || booking.AccountId != accountIdString)
            {
                return NotFound();
            }

            try
            {
                // Cancel the booking
                bool result = _bookingService.CancelBooking(id);

                if (result)
                {
                    TempData["SuccessMessage"] = "Booking has been successfully cancelled.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to cancel booking. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error cancelling booking: {ex.Message}";
            }

            return Page();
        }

        public string GetStatusBadgeClass()
        {
            return Booking.Status switch
            {
                "pending" => "bg-warning text-dark",
                "accepted" => "bg-primary",
                "rejected" => "bg-danger",
                "in-progress" => "bg-info",
                "completed" => "bg-success",
                "canceled" => "bg-secondary",
                _ => "bg-secondary"
            };
        }

        public string GetRecordStatusBadgeClass()
        {
            if (Record == null) return string.Empty;

            return Record.Status switch
            {
                "Accepted" => "bg-primary",
                "InProgress" => "bg-info",
                "Finished" => "bg-success",
                _ => "bg-secondary"
            };
        }
    }
}