using System;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Caregiver.Records
{
    public class UpdateModel : PageModel
    {
        private readonly IRecordService _recordService;
        private readonly IBookingService _bookingService;
        private readonly IElderService _elderService;
        private readonly ICaregiverService _caregiverService;

        public UpdateModel(
            IRecordService recordService,
            IBookingService bookingService,
            IElderService elderService,
            ICaregiverService caregiverService)
        {
            _recordService = recordService;
            _bookingService = bookingService;
            _elderService = elderService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public Record Record { get; set; }

        public Elder Elder { get; set; }
        public Booking Booking { get; set; }
        public string StatusMessage { get; set; }

        public IActionResult OnGet(int id, string action = null)
        {
            // Check if user is logged in and is a caregiver
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                // Get caregiver ID
                var caregiver = _caregiverService.GetCaregiverByAccountId(accountId.Value);
                if (caregiver == null)
                {
                    return RedirectToPage("/Auth/Login");
                }

                // Get the record
                Record = _recordService.GetRecordById(id);
                if (Record == null)
                {
                    return NotFound();
                }

                // Get the booking
                Booking = _bookingService.GetBookingById(Record.BookingId);

                // Ensure this record belongs to the logged-in caregiver
                if (Booking != null && Booking.CaregiverId != caregiver.CaregiverId)
                {
                    return Forbid();
                }

                // Get the elder
                Elder = _elderService.GetElderById(Record.ElderId);

                // Handle direct actions (clock in only)
                if (!string.IsNullOrEmpty(action))
                {
                    if (action.Equals("clockin", StringComparison.OrdinalIgnoreCase) && Record.Status == "Accepted")
                    {
                        // Update record status
                        _recordService.ClockIn(id);

                        // Update booking status to "in-progress"
                        Booking.Status = "in-progress";
                        _bookingService.UpdateBooking(Booking);

                        StatusMessage = "Successfully clocked in. Service is now in progress.";
                        return RedirectToPage("./Index");
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPost(string action = null)
        {
            // Check if user is logged in and is a caregiver
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Caregiver")
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                // Get caregiver ID
                var caregiver = _caregiverService.GetCaregiverByAccountId(accountId.Value);
                if (caregiver == null)
                {
                    return RedirectToPage("/Auth/Login");
                }

                // Get the original record
                var originalRecord = _recordService.GetRecordById(Record.RecordId);
                if (originalRecord == null)
                {
                    return NotFound();
                }

                // Get the booking
                var booking = _bookingService.GetBookingById(originalRecord.BookingId);

                // Ensure this record belongs to the logged-in caregiver
                if (booking != null && booking.CaregiverId != caregiver.CaregiverId)
                {
                    return Forbid();
                }

                // Check if this is a clock out action
                if (action == "clockout")
                {
                    // Validate that all guidelines are filled out
                    if (string.IsNullOrWhiteSpace(Record.ExerciseGuidelines) ||
                        string.IsNullOrWhiteSpace(Record.DietGuidelines) ||
                        string.IsNullOrWhiteSpace(Record.OtherGuidelines))
                    {
                        ModelState.AddModelError("", "All guidelines (Exercise, Diet, and Other) must be filled out before clocking out.");
                        Elder = _elderService.GetElderById(originalRecord.ElderId);
                        Booking = booking;
                        return Page();
                    }

                    // Update the guidelines first
                    _recordService.UpdateGuidelines(
                        Record.RecordId,
                        Record.ExerciseGuidelines,
                        Record.DietGuidelines,
                        Record.OtherGuidelines
                    );

                    // Clock out the record
                    _recordService.ClockOut(Record.RecordId);

                    // Update booking status to "completed"
                    booking.Status = "completed";
                    _bookingService.UpdateBooking(booking);

                    StatusMessage = "Service completed successfully. You have clocked out.";
                    return RedirectToPage("./Index");
                }
                else
                {
                    // Regular update action - just update the guidelines
                    _recordService.UpdateGuidelines(
                        Record.RecordId,
                        Record.ExerciseGuidelines,
                        Record.DietGuidelines,
                        Record.OtherGuidelines
                    );

                    StatusMessage = "Record guidelines updated successfully.";
                    return RedirectToPage("./Details", new { id = Record.RecordId });
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}