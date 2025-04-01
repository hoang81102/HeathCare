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
    public class IndexModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly ICaregiverService _caregiverService;

        public IndexModel(IBookingService bookingService, ICaregiverService caregiverService)
        {
            _bookingService = bookingService;
            _caregiverService = caregiverService;
        }

        public List<Booking> Bookings { get; set; } = new List<Booking>();

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "all";

        public IActionResult OnGet()
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

            // Load bookings based on filter
            switch (Filter.ToLower())
            {
                case "pending":
                    Bookings = _bookingService.GetBookingsByCaregiverId(caregiver.CaregiverId)
                        .Where(b => b.Status == "pending")
                        .OrderByDescending(b => b.BookingDateTime)
                        .ToList();
                    break;

                case "upcoming":
                    Bookings = _bookingService.GetUpcomingBookingsByCaregiverId(caregiver.CaregiverId);
                    break;

                case "completed":
                    Bookings = _bookingService.GetBookingsByCaregiverId(caregiver.CaregiverId)
                        .Where(b => b.Status == "completed")
                        .OrderByDescending(b => b.BookingDateTime)
                        .ToList();
                    break;

                case "all":
                default:
                    Bookings = _bookingService.GetBookingsByCaregiverId(caregiver.CaregiverId)
                        .OrderByDescending(b => b.BookingDateTime)
                        .ToList();
                    break;
            }

            return Page();
        }
    }
}