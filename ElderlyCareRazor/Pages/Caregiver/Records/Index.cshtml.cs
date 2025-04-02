using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Caregiver.Records
{
    public class IndexModel : PageModel
    {
        private readonly IRecordService _recordService;
        private readonly IBookingService _bookingService;
        private readonly IElderService _elderService;
        private readonly ICaregiverService _caregiverService;

        public IndexModel(
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

        public List<Record> Records { get; set; } = new List<Record>();
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public Dictionary<int, Elder> ElderDict { get; set; } = new Dictionary<int, Elder>();
        public string StatusMessage { get; set; }

        public IActionResult OnGet()
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

                // Get records associated with this caregiver
                Records = _recordService.GetRecordsByCaregiverId(caregiver.CaregiverId);

                // Get related bookings
                var bookingIds = Records.Select(r => r.BookingId).Distinct().ToList();
                foreach (var bookingId in bookingIds)
                {
                    var booking = _bookingService.GetBookingById(bookingId);
                    if (booking != null)
                    {
                        Bookings.Add(booking);
                    }
                }

                // Get related elders
                var elderIds = Records.Select(r => r.ElderId).Distinct().ToList();
                foreach (var elderId in elderIds)
                {
                    var elder = _elderService.GetElderById(elderId);
                    if (elder != null)
                    {
                        ElderDict[elderId] = elder;
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
    }
}