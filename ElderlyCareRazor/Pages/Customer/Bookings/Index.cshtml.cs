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
    public class IndexModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingTimeSlotService _bookingTimeSlotService;
        private readonly IServiceService _serviceService;
        private readonly IElderService _elderService;
        private readonly ICaregiverService _caregiverService;
        private readonly IFeedbackService _feedbackService;

        public IndexModel(
            IBookingService bookingService,
            IBookingTimeSlotService bookingTimeSlotService,
            IServiceService serviceService,
            IElderService elderService,
            ICaregiverService caregiverService,
            IFeedbackService feedbackService)
        {
            _bookingService = bookingService;
            _bookingTimeSlotService = bookingTimeSlotService;
            _serviceService = serviceService;
            _elderService = elderService;
            _caregiverService = caregiverService;
            _feedbackService = feedbackService;
        }

        public List<Booking> AllBookings { get; set; }
        public List<Booking> UpcomingBookings { get; set; }
        public List<Booking> PendingBookings { get; set; }
        public List<Booking> PastBookings { get; set; }
        public List<Booking> CancelledBookings { get; set; }

        public Dictionary<int, BookingTimeSlot> BookingTimeSlots { get; set; } = new Dictionary<int, BookingTimeSlot>();
        public Dictionary<int, Service> Services { get; set; }
        public Dictionary<int, Elder> Elders { get; set; }
        public Dictionary<int, BusinessObjects.Caregiver> Caregivers { get; set; }
        public HashSet<int> BookingsWithFeedback { get; set; }

        public IActionResult OnGet()
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
            InitializeTimeSlots(accountId.Value);

            // Load all bookings for this customer
            LoadBookings(accountId.Value);

            // Load related data
            LoadRelatedData();

            return Page();
        }
        private void InitializeTimeSlots(int accountId)
        {
            // Clear existing dictionary
            BookingTimeSlots.Clear();

            // Get all bookings for the customer
            var bookings = _bookingService.GetBookingsByAccountId(accountId);

            // For each booking, get the first time slot and add it to the dictionary
            foreach (var booking in bookings)
            {
                var timeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(booking.BookingId);
                if (timeSlots.Any())
                {
                    BookingTimeSlots[booking.BookingId] = timeSlots.First();
                }
            }
        }
        private void LoadBookings(int accountId)
        {
            // Get all bookings for the customer
            AllBookings = _bookingService.GetBookingsByAccountId(accountId);

            // Filter bookings by status
            DateTime today = DateTime.Today;

            // Upcoming bookings - Accepted bookings with future dates
            UpcomingBookings = AllBookings
                .Where(b => b.Status == "accepted" &&
                           (b.BookingDateTime > today ||
                           (b.BookingDateTime.Date == today && DateTime.Now.TimeOfDay < GetBookingStartTime(b.BookingId))))
                .OrderBy(b => b.BookingDateTime)
                .ToList();

            // Pending bookings - Pending status
            PendingBookings = AllBookings
                .Where(b => b.Status == "pending")
                .OrderBy(b => b.BookingDateTime)
                .ToList();

            // Past bookings - Completed bookings or past dates
            PastBookings = AllBookings
                .Where(b => (b.Status == "completed" || b.Status == "rejected" ||
                           (b.Status == "accepted" && b.BookingDateTime.Date < today) ||
                           (b.Status == "accepted" && b.BookingDateTime.Date == today && DateTime.Now.TimeOfDay > GetBookingEndTime(b.BookingId))))
                .OrderByDescending(b => b.BookingDateTime)
                .ToList();

            // Cancelled bookings
            CancelledBookings = AllBookings
                .Where(b => b.Status == "canceled")
                .OrderByDescending(b => b.BookingDateTime)
                .ToList();
        }

        private void LoadRelatedData()
        {
            // Initialize dictionaries
            BookingTimeSlots = new Dictionary<int, BookingTimeSlot>();
            Services = new Dictionary<int, Service>();
            Elders = new Dictionary<int, Elder>();
            Caregivers = new Dictionary<int, BusinessObjects.Caregiver>();
            BookingsWithFeedback = new HashSet<int>();

            // Load all time slots
            foreach (var booking in AllBookings)
            {
                var timeSlots = _bookingTimeSlotService.GetTimeSlotsByBookingId(booking.BookingId);
                if (timeSlots.Any())
                {
                    BookingTimeSlots[booking.BookingId] = timeSlots.First();
                }

                // Load service if not already loaded
                if (!Services.ContainsKey(booking.ServiceId))
                {
                    var service = _serviceService.GetServiceById(booking.ServiceId);
                    if (service != null)
                    {
                        Services[booking.ServiceId] = service;
                    }
                }

                // Load elder if not already loaded
                if (booking.ElderId.HasValue && !Elders.ContainsKey(booking.ElderId.Value))
                {
                    var elder = _elderService.GetElderById(booking.ElderId.Value);
                    if (elder != null)
                    {
                        Elders[booking.ElderId.Value] = elder;
                    }
                }

                // Load caregiver if not already loaded
                if (!Caregivers.ContainsKey(booking.CaregiverId))
                {
                    var caregiver = _caregiverService.GetCaregiverById(booking.CaregiverId);
                    if (caregiver != null)
                    {
                        Caregivers[booking.CaregiverId] = caregiver;
                    }
                }

                // Check if feedback exists
                var feedbacks = _feedbackService.GetFeedbacksByBookingId(booking.BookingId);
                if (feedbacks.Any())
                {
                    BookingsWithFeedback.Add(booking.BookingId);
                }
            }
        }

        public IActionResult OnPostCancelBooking(int id)
        {
            // Check if user is logged in
            var accountIdString = HttpContext.Session.GetInt32("AccountId");
            if (accountIdString == null || HttpContext.Session.GetString("Role") != "Customer")
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get booking details
            var booking = _bookingService.GetBookingById(id);

            // Check if booking exists and belongs to the current user
            if (booking == null || booking.AccountId != accountIdString)
            {
                TempData["ErrorMessage"] = "Booking not found or you don't have permission to cancel it.";
                return RedirectToPage();
            }

            try
            {
                // Check if booking can be cancelled
                if (booking.Status != "pending" && booking.Status != "accepted")
                {
                    TempData["ErrorMessage"] = "Only pending or accepted bookings can be cancelled.";
                    return RedirectToPage();
                }

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

            return RedirectToPage();
        }

        public DateOnly? GetBookingDate(int bookingId)
        {
            if (BookingTimeSlots.TryGetValue(bookingId, out var timeSlot))
            {
                return timeSlot.BookingDate;
            }
            return null;
        }

        public string GetBookingTime(int bookingId)
        {
            if (BookingTimeSlots.TryGetValue(bookingId, out var timeSlot))
            {
                return $"{timeSlot.StartTime.ToString(@"hh\:mm tt")} - {timeSlot.EndTime.ToString(@"hh\:mm tt")}";
            }
            return null;
        }

        private TimeSpan GetBookingStartTime(int bookingId)
        {
            if (BookingTimeSlots.TryGetValue(bookingId, out var timeSlot))
            {
                return TimeSpan.FromHours(timeSlot.StartTime.Hour).Add(TimeSpan.FromMinutes(timeSlot.StartTime.Minute));
            }
            return TimeSpan.Zero;
        }

        private TimeSpan GetBookingEndTime(int bookingId)
        {
            if (BookingTimeSlots.TryGetValue(bookingId, out var timeSlot))
            {
                return TimeSpan.FromHours(timeSlot.EndTime.Hour).Add(TimeSpan.FromMinutes(timeSlot.EndTime.Minute));
            }
            return TimeSpan.Zero;
        }

        public string GetServiceName(int serviceId)
        {
            if (Services.TryGetValue(serviceId, out var service))
            {
                return service.ServiceName;
            }
            return "Unknown Service";
        }

        public string GetElderName(int? elderId)
        {
            if (elderId.HasValue && Elders.TryGetValue(elderId.Value, out var elder))
            {
                return elder.Fullname;
            }
            return "Unknown Elder";
        }

        public string GetCaregiverName(int caregiverId)
        {
            if (Caregivers.TryGetValue(caregiverId, out var caregiver))
            {
                return caregiver.Account?.Fullname ?? "Unknown Caregiver";
            }
            return "Unknown Caregiver";
        }

        public bool HasFeedback(int bookingId)
        {
            return BookingsWithFeedback.Contains(bookingId);
        }

        public string GetStatusBadgeClass(string status)
        {
            return status switch
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

        public bool CanBeCancelled(Booking booking)
        {
            // A booking can be cancelled if:
            // 1. It's in pending or accepted status
            // 2. It's not in the past
            // 3. It's not already in progress or completed or cancelled

            if (booking.Status != "pending" && booking.Status != "accepted")
            {
                return false;
            }

            if (BookingTimeSlots.TryGetValue(booking.BookingId, out var timeSlot))
            {
                // If the booking date is in the past, it can't be cancelled
                if (timeSlot.BookingDate < DateOnly.FromDateTime(DateTime.Today))
                {
                    return false;
                }

                // If the booking is today, check if the start time has passed
                if (timeSlot.BookingDate == DateOnly.FromDateTime(DateTime.Today))
                {
                    var currentTime = TimeOnly.FromDateTime(DateTime.Now);
                    if (timeSlot.StartTime <= currentTime)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}