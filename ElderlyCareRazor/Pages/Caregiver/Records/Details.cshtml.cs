using System;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Caregiver.Records
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
        public MedicalRecord LatestMedicalRecord { get; set; }
        public string StatusMessage { get; set; }

        public IActionResult OnGet(int id)
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

                // Get the latest medical record for this elder
                LatestMedicalRecord = _medicalRecordService.GetLatestMedicalRecord(Record.ElderId);

                return Page();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return Page();
            }
        }
    }
}