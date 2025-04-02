using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Caregiver.Bookings
{
    public class ManageStatusModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingTimeSlotService _bookingTimeSlotService;
        private readonly ICaregiverService _caregiverService;
        private readonly IRecordService _recordService;

        public ManageStatusModel(
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

        [BindProperty]
        public string Decision { get; set; }

        [BindProperty]
        [StringLength(500)]
        [Display(Name = "Rejection Reason")]
        public string RejectionReason { get; set; }

        public IActionResult OnGet(int id, bool? refresh = null)
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

            // Apply no-cache directive to ensure fresh data
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Get booking details - always get fresh data from the database
            Booking = _bookingService.GetBookingById(id);

            // Check if booking exists and belongs to this caregiver
            if (Booking == null || Booking.CaregiverId != caregiver.CaregiverId)
            {
                return NotFound();
            }

            // Get time slots for this booking
            TimeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(id);
            Console.WriteLine($"Booking Status: {Booking?.Status}");

            return Page();
        }

        public IActionResult OnPost()
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

            int bookingId = int.Parse(Request.Form["Booking.BookingId"]);

            // Get booking details - use fresh data from the database
            Booking = _bookingService.GetBookingById(bookingId);

            // Check if booking exists and belongs to this caregiver
            if (Booking == null || Booking.CaregiverId != caregiver.CaregiverId)
            {
                return NotFound();
            }

            // Check if booking is in pending status
            if (Booking.Status != "pending")
            {
                TempData["ErrorMessage"] = "This booking has already been reviewed.";
                return RedirectToPage("./Details", new { id = bookingId });
            }

            // Process the decision
            if (Decision == "Accept")
            {
                // Accept the booking
                bool success = _bookingService.AcceptBooking(bookingId);
                if (success)
                {
                    // Force immediate refresh of the booking data
                    Booking = _bookingService.GetBookingById(bookingId);

                    TempData["SuccessMessage"] = "Booking has been accepted successfully.";

                    // Add cache-busting parameter and use PRG pattern properly
                    return RedirectToPage("./Details", new { id = bookingId, noCache = DateTime.Now.Ticks });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to accept the booking. Please try again.");
                    // Reload time slots
                    TimeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(bookingId);
                    return Page();
                }
            }
            else if (Decision == "Reject")
            {
                // Validate rejection reason
                if (string.IsNullOrWhiteSpace(RejectionReason))
                {
                    ModelState.AddModelError("RejectionReason", "Please provide a reason for rejection.");
                    // Reload time slots
                    TimeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(bookingId);
                    return Page();
                }

                // Reject the booking
                bool success = _bookingService.RejectBooking(bookingId, RejectionReason);
                if (success)
                {
                    // Force immediate refresh of the booking data
                    Booking = _bookingService.GetBookingById(bookingId);

                    TempData["SuccessMessage"] = "Booking has been rejected.";

                    // Add cache-busting parameter
                    return RedirectToPage("./Details", new { id = bookingId, noCache = DateTime.Now.Ticks });
                }
                else
                {
                    ModelState.AddModelError("", "Failed to reject the booking. Please try again.");
                    // Reload time slots
                    TimeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(bookingId);
                    return Page();
                }
            }

            // If we get here, something went wrong
            ModelState.AddModelError("", "Invalid decision. Please try again.");
            // Reload time slots
            TimeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(bookingId);
            return Page();
        }
    }
}