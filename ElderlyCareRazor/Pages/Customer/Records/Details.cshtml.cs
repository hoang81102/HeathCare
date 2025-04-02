using System;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Customer.Records
{
    public class DetailsModel : PageModel
    {
        private readonly IRecordService _recordService;
        private readonly IBookingService _bookingService;
        private readonly IElderService _elderService;
        private readonly ICaregiverService _caregiverService;
        private readonly IMedicalRecordService _medicalRecordService;

        public DetailsModel(
            IRecordService recordService,
            IBookingService bookingService,
            IElderService elderService,
            ICaregiverService caregiverService,
            IMedicalRecordService medicalRecordService)
        {
            _recordService = recordService;
            _bookingService = bookingService;
            _elderService = elderService;
            _caregiverService = caregiverService;
            _medicalRecordService = medicalRecordService;
        }

        public Record Record { get; set; }
        public Booking Booking { get; set; }
        public Elder Elder { get; set; }
        public BusinessObjects.Caregiver Caregiver { get; set; }
        public MedicalRecord LatestMedicalRecord { get; set; }
        public string StatusMessage { get; set; }
        public bool CanLeaveFeedback { get; set; }

        public IActionResult OnGet(int id, int? bookingId)
        {
            // Check if user is logged in and is a customer
            var accountId = HttpContext.Session.GetInt32("AccountId");
            var role = HttpContext.Session.GetString("Role");

            if (accountId == null || role != "Customer")
            {
                return RedirectToPage("/Auth/Login");
            }

            try
            {
                // If bookingId is provided, try to find the record associated with it
                if (bookingId.HasValue)
                {
                    Record = _recordService.GetRecordByBookingId(bookingId.Value);
                    if (Record == null)
                    {
                        StatusMessage = "No record found for this booking.";
                        // Continue to show booking details even if record is not found
                        Booking = _bookingService.GetBookingById(bookingId.Value);
                    }
                    else
                    {
                        id = Record.RecordId;
                    }
                }
                else
                {
                    // Get the record by ID
                    Record = _recordService.GetRecordById(id);
                }

                if (Record == null && !bookingId.HasValue)
                {
                    return NotFound();
                }

                // If record exists, get associated data
                if (Record != null)
                {
                    // Get the booking
                    Booking = _bookingService.GetBookingById(Record.BookingId);

                    // Ensure this booking belongs to the logged-in customer
                    if (Booking != null && Booking.AccountId != accountId.Value)
                    {
                        return Forbid();
                    }

                    // Get the elder
                    Elder = _elderService.GetElderById(Record.ElderId);

                    // Get the caregiver
                    Caregiver = _caregiverService.GetCaregiverById(Booking.CaregiverId);

                    // Get the latest medical record for this elder
                    LatestMedicalRecord = _medicalRecordService.GetLatestMedicalRecord(Record.ElderId);
                }
                else if (Booking != null)
                {
                    // Still verify that booking belongs to customer
                    if (Booking.AccountId != accountId.Value)
                    {
                        return Forbid();
                    }

                    // Get related info from booking
                    Elder = _elderService.GetElderById(Booking.ElderId ?? 0);
                    Caregiver = _caregiverService.GetCaregiverById(Booking.CaregiverId);
                }
                else
                {
                    return NotFound();
                }

                // Check if customer can leave feedback for this booking
                CanLeaveFeedback = (Booking != null &&
                                    Booking.Status.Equals("completed", StringComparison.OrdinalIgnoreCase) &&
                                    !HasFeedback(Booking.BookingId));

                return Page();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return Page();
            }
        }

        private bool HasFeedback(int bookingId)
        {
            // This would need a service method to check if feedback exists
            // For now, return false to always show the feedback option
            return false;
        }
    }
}