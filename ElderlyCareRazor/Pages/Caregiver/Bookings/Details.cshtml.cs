using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Caregiver.Bookings
{
    public class DetailsModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingTimeSlotService _bookingTimeSlotService;
        private readonly ICaregiverService _caregiverService;
        private readonly IRecordService _recordService;

        public DetailsModel(
            IBookingService bookingService,
            IBookingTimeSlotService bookingTimeSlotService,
            ICaregiverService caregiverService,
            IRecordService recordService)
        {
            _bookingService = bookingService;
            _bookingTimeSlotService = bookingTimeSlotService;
            _caregiverService = caregiverService;
            _recordService = recordService;
        }

        public Booking Booking { get; set; }
        public List<BookingTimeSlot> TimeSlots { get; set; } = new List<BookingTimeSlot>();
        public Record Record { get; set; }

        public IActionResult OnGet(int id, long? noCache = null)
        {
            // Check if user is logged in and is a caregiver
            if (HttpContext.Session.GetString("Role") != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }

            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get caregiver ID based on account ID
            var caregiver = _caregiverService.GetCaregiverByAccountId(accountId.Value);
            if (caregiver == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Add no-cache headers to prevent browser caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Get booking details directly from the database without any caching
            Booking = _bookingService.GetBookingById(id);

            // Double check if the booking status is up to date
            if (Booking != null)
            {
                // Ensure we're getting the latest data by re-fetching
                var freshBooking = _bookingService.GetBookingById(id);
                if (freshBooking != null && Booking.Status != freshBooking.Status)
                {
                    Booking = freshBooking;
                }
            }

            // Check if booking exists and belongs to this caregiver
            if (Booking == null || Booking.CaregiverId != caregiver.CaregiverId)
            {
                return NotFound();
            }

            // Get time slots for this booking
            TimeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(id);

            // Check if there's a record for this booking
            Record = _recordService.GetRecordByBookingId(id);

            // Handle any success messages
            if (TempData["SuccessMessage"] != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"]?.ToString();
            }

            return Page();
        }
    }
}