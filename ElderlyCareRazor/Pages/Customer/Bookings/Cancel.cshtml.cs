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
    public class CancelModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingTimeSlotService _bookingTimeSlotService;
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IElderService _elderService;
        private readonly ICaregiverService _caregiverService;

        public CancelModel(
            IBookingService bookingService,
            IBookingTimeSlotService bookingTimeSlotService,
            IServiceService serviceService,
            IServiceCategoryService serviceCategoryService,
            IElderService elderService,
            ICaregiverService caregiverService)
        {
            _bookingService = bookingService;
            _bookingTimeSlotService = bookingTimeSlotService;
            _serviceService = serviceService;
            _serviceCategoryService = serviceCategoryService;
            _elderService = elderService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public int BookingId { get; set; }

        public Booking Booking { get; set; }
        public BookingTimeSlot BookingTimeSlot { get; set; }
        public Service Service { get; set; }
        public ServiceCategory ServiceCategory { get; set; }
        public Elder Elder { get; set; }
        public BusinessObjects.Caregiver Caregiver { get; set; }
        public bool CanBeCancelled { get; set; }
        public bool CancellationSuccessful { get; set; }

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

            BookingId = id;

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

            // If booking cannot be cancelled, redirect to details page
            if (!CanBeCancelled)
            {
                TempData["ErrorMessage"] = "This booking cannot be cancelled.";
                return RedirectToPage("Details", new { id = BookingId });
            }

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
                if (BookingTimeSlot.BookingDate == DateOnly.FromDateTime(DateTime.Today))
                {
                    var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                    if (BookingTimeSlot.StartTime <= currentTime)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public IActionResult OnPost()
        {
            // Check if user is logged in
            var accountIdString = HttpContext.Session.GetInt32("AccountId");
            if (accountIdString == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get booking details
            var booking = _bookingService.GetBookingById(BookingId);

            // Check if booking exists and belongs to the current user
            if (booking == null || booking.AccountId != accountIdString)
            {
                TempData["ErrorMessage"] = "Booking not found or you don't have permission to cancel it.";
                return RedirectToPage("Index");
            }

            try
            {
                // Check if booking can be cancelled
                if (booking.Status != "pending" && booking.Status != "accepted")
                {
                    TempData["ErrorMessage"] = "Only pending or accepted bookings can be cancelled.";
                    return RedirectToPage("Details", new { id = BookingId });
                }

                // Load time slot to check date/time
                var timeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(BookingId);
                var timeSlot = timeSlots.FirstOrDefault();

                if (timeSlot != null)
                {
                    // If the booking date is in the past, it can't be cancelled
                    if (timeSlot.BookingDate < DateOnly.FromDateTime(DateTime.Today))
                    {
                        TempData["ErrorMessage"] = "Past bookings cannot be cancelled.";
                        return RedirectToPage("Details", new { id = BookingId });
                    }

                    // If the booking is today and the start time has passed, it can't be cancelled
                    if (timeSlot.BookingDate == DateOnly.FromDateTime(DateTime.Today))
                    {
                        var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                        if (timeSlot.StartTime <= currentTime)
                        {
                            TempData["ErrorMessage"] = "Bookings that have already started cannot be cancelled.";
                            return RedirectToPage("Details", new { id = BookingId });
                        }
                    }
                }

                // Cancel the booking
                bool result = _bookingService.CancelBooking(BookingId);

                if (result)
                {
                    // After successfully cancelling, fetch the updated booking to reflect changes
                    CancellationSuccessful = true;
                    TempData["SuccessMessage"] = "Booking has been successfully cancelled.";

                    // Force a redirect to the Index page with a timestamp parameter to prevent caching
                    return RedirectToPage("Index", new { t = DateTime.Now.Ticks });
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to cancel booking. Please try again.";
                    return RedirectToPage("Details", new { id = BookingId });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error cancelling booking: {ex.Message}";
                return RedirectToPage("Details", new { id = BookingId });
            }
        }
    }
}