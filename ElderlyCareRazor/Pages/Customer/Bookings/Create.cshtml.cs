using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using Services;
using Microsoft.AspNetCore.Http;
using DataAccessObjects;

namespace ElderlyCareRazor.Pages.Customer.Bookings
{
    public class CreateModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IElderService _elderService;
        private readonly IBookingService _bookingService;
        private readonly ICaregiverService _caregiverService;

        public CreateModel(
            IServiceService serviceService,
            IServiceCategoryService serviceCategoryService,
            IElderService elderService,
            IBookingService bookingService,
            ICaregiverService caregiverService)
        {
            _serviceService = serviceService;
            _serviceCategoryService = serviceCategoryService;
            _elderService = elderService;
            _bookingService = bookingService;
            _caregiverService = caregiverService;
        }

        [BindProperty]
        public Booking Booking { get; set; }

        [BindProperty]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [BindProperty]
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0); // Default to 9:00 AM

        [BindProperty]
        public TimeSpan EndTime { get; set; } = new TimeSpan(10, 0, 0); // Default to 10:00 AM

        public Service Service { get; set; }
        public ServiceCategory ServiceCategory { get; set; }
        public List<Elder> Elders { get; set; }
        public List<CaregiverAvailabilityResult> AvailableCaregivers { get; set; }
        public bool SearchPerformed { get; set; } = false;

        public IActionResult OnGet(int? serviceId)
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

            // Initialize the booking with the current user's account ID
            Booking = new Booking
            {
                AccountId = accountId.Value,
                BookingDateTime = DateTime.Now,
                Status = "pending"
            };

            // If service ID is provided, load service details
            if (serviceId.HasValue)
            {
                Service = _serviceService.GetServiceById(serviceId.Value);
                if (Service == null)
                {
                    return RedirectToPage("/Customer/Dashboard");
                }

                Booking.ServiceId = serviceId.Value;
                ServiceCategory = _serviceCategoryService.GetCategoryById(Service.CategoryId);
            }
            else
            {
                return RedirectToPage("/Customer/Dashboard");
            }

            // Load elders for the current user
            Elders = _elderService.GetEldersByAccountId(Booking.AccountId);

            return Page();
        }

        public IActionResult OnGetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime, int elderId)
        {
            SearchPerformed = true;

            // Validate inputs
            if (startTime >= endTime)
            {
                return Content("<div class='alert alert-danger'>Start time must be before end time.</div>");
            }

            if (bookingDate.Date < DateTime.Today)
            {
                return Content("<div class='alert alert-danger'>Booking date cannot be in the past.</div>");
            }

            // Get available caregivers
            try
            {
                AvailableCaregivers = _bookingService.GetAvailableCaregivers(bookingDate, startTime, endTime);

                if (AvailableCaregivers == null || !AvailableCaregivers.Any())
                {
                    return Content("<div class='alert alert-info'>No caregivers available for the selected time. Please try a different date or time.</div>");
                }

                var content = "<div class='mb-3'>" +
                              "<label for='Booking_CaregiverId' class='form-label'>Caregiver</label>" +
                              "<select id='Booking_CaregiverId' name='Booking.CaregiverId' class='form-select' required>" +
                              "<option value=''>-- Select Caregiver --</option>";

                foreach (var caregiver in AvailableCaregivers)
                {
                    content += $"<option value='{caregiver.CaregiverId}'>{caregiver.Fullname}</option>";
                }

                content += "</select>" +
                          "<span class='text-danger' data-valmsg-for='Booking.CaregiverId' data-valmsg-replace='true'></span>" +
                          "</div>";

                return Content(content);
            }
            catch (Exception ex)
            {
                return Content($"<div class='alert alert-danger'>Error finding available caregivers: {ex.Message}</div>");
            }
        }

        public IActionResult OnPost()
        {
            // Check if user is logged in
            var accountIdString = HttpContext.Session.GetInt32("AccountId");
            if (accountIdString == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToPage("/Auth/Login");
            }

            // Validate inputs
            if (StartTime >= EndTime)
            {
                ModelState.AddModelError("", "Start time must be before end time.");
                return Page();
            }

            if (BookingDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "Booking date cannot be in the past.");
                return Page();
            }

            ModelState.Remove("Booking.Status");
            ModelState.Remove("Booking.Account");
            ModelState.Remove("Booking.Service");
            ModelState.Remove("Booking.Caregiver");
            ModelState.Remove("Booking.Elder");

            if (!ModelState.IsValid)
            {
                // Reload required data for the page
                Service = _serviceService.GetServiceById(Booking.ServiceId);
                ServiceCategory = Service != null ? _serviceCategoryService.GetCategoryById(Service.CategoryId) : null;
                Elders = _elderService.GetEldersByAccountId(Booking.AccountId);
                return Page();
            }

            try
            {
                // Fix for the stored procedure with correct parameter count
                int bookingId = _bookingService.CreateBooking(
                    Booking.AccountId,
                    Booking.ServiceId,
                    Booking.CaregiverId,
                    Booking.ElderId.Value,
                    BookingDate,
                    StartTime,
                    EndTime
                );

                // Redirect to the booking details page
                return RedirectToPage("Details", new { id = bookingId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating booking: {ex.Message}");

                // Reload required data for the page
                Service = _serviceService.GetServiceById(Booking.ServiceId);
                ServiceCategory = Service != null ? _serviceCategoryService.GetCategoryById(Service.CategoryId) : null;
                Elders = _elderService.GetEldersByAccountId(Booking.AccountId);
                return Page();
            }
        }
    }
}