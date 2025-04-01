using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Customer
{
    public class DashboardModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IAccountService _accountService;
        private readonly IElderService _elderService;
        private readonly IBookingService _bookingService;
        private readonly IFeedbackService _feedbackService;
        private readonly ICaregiverService _caregiverService;
        private readonly IRecordService _recordService; // Added RecordService

        public DashboardModel(
            IServiceService serviceService,
            IServiceCategoryService serviceCategoryService,
            IAccountService accountService,
            IElderService elderService,
            IBookingService bookingService,
            IFeedbackService feedbackService,
            ICaregiverService caregiverService,
            IRecordService recordService) // Added RecordService parameter
        {
            _serviceService = serviceService;
            _serviceCategoryService = serviceCategoryService;
            _accountService = accountService;
            _elderService = elderService;
            _bookingService = bookingService;
            _feedbackService = feedbackService;
            _caregiverService = caregiverService;
            _recordService = recordService; // Initialize RecordService
        }

        public List<Service> Services { get; set; } = new List<Service>();
        public List<ServiceCategory> ServiceCategories { get; set; } = new List<ServiceCategory>();
        public Account CurrentAccount { get; set; }
        public List<Elder> Elders { get; set; } = new List<Elder>();
        public List<Booking> UpcomingBookings { get; set; } = new List<Booking>();
        public List<Booking> CompletedBookings { get; set; } = new List<Booking>();
        public List<Record> ElderRecords { get; set; } = new List<Record>(); // Added ElderRecords property

        // Dictionaries to cache elder and caregiver names to avoid repeated lookups
        private Dictionary<int, string> _elderNames = new Dictionary<int, string>();
        private Dictionary<int, string> _caregiverNames = new Dictionary<int, string>();
        private Dictionary<int, int> _bookingToCaregiverId = new Dictionary<int, int>(); // To map bookings to caregivers
        private Dictionary<int, string> _bookingToServiceName = new Dictionary<int, string>(); // To map bookings to service names
        private HashSet<int> _bookingsWithFeedback = new HashSet<int>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Get current user ID from session
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Load the current account
            CurrentAccount = await _accountService.GetAccountByIdAsync(accountId.Value);
            if (CurrentAccount == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Load services and categories
            Services = _serviceService.GetAllServices();
            ServiceCategories = _serviceCategoryService.GetAllCategories();

            // Load elders for this account
            Elders = _elderService.GetEldersByAccountId(accountId.Value);

            // Load upcoming and completed bookings
            UpcomingBookings = _bookingService.GetUpcomingBookingsByAccountId(accountId.Value);
            CompletedBookings = _bookingService.GetCompletedBookingsByAccountId(accountId.Value);

            // Load records for all elders associated with this account
            ElderRecords = new List<Record>();
            foreach (var elder in Elders)
            {
                var elderRecords = _recordService.GetRecordsByElderId(elder.ElderId);
                if (elderRecords != null && elderRecords.Any())
                {
                    ElderRecords.AddRange(elderRecords);
                }
            }

            // Sort records by last update date (newest first)
            ElderRecords = ElderRecords.OrderByDescending(r => r.LastUpdated).ToList();

            // Pre-cache elder names
            foreach (var elder in Elders)
            {
                _elderNames[elder.ElderId] = elder.Fullname;
            }

            // Pre-cache all booking information
            var allBookings = _bookingService.GetAllBookings();
            foreach (var booking in allBookings)
            {
                // Cache caregiver IDs for each booking
                _bookingToCaregiverId[booking.BookingId] = booking.CaregiverId;

                // Cache service names for each booking
                if (booking.Service != null)
                {
                    _bookingToServiceName[booking.BookingId] = booking.Service.ServiceName;
                }
            }

            // Pre-cache caregiver names
            var caregiverIds = UpcomingBookings.Select(b => b.CaregiverId)
                .Union(CompletedBookings.Select(b => b.CaregiverId))
                .Union(_bookingToCaregiverId.Values)
                .Distinct()
                .ToList();

            foreach (var caregiverId in caregiverIds)
            {
                var caregiver = _caregiverService.GetCaregiverById(caregiverId);
                if (caregiver != null)
                {
                    var caregiverAccount = await _accountService.GetAccountByIdAsync(caregiver.AccountId);
                    if (caregiverAccount != null)
                    {
                        _caregiverNames[caregiverId] = caregiverAccount.Fullname;
                    }
                }
            }

            // Pre-cache bookings with feedback
            var bookingIds = CompletedBookings.Select(b => b.BookingId).ToList();
            foreach (var bookingId in bookingIds)
            {
                var feedbacks = _feedbackService.GetFeedbacksByBookingId(bookingId);
                if (feedbacks != null && feedbacks.Any())
                {
                    _bookingsWithFeedback.Add(bookingId);
                }
            }

            return Page();
        }

        public string GetElderName(int? elderId)
        {
            if (!elderId.HasValue)
            {
                return "N/A";
            }

            if (_elderNames.ContainsKey(elderId.Value))
            {
                return _elderNames[elderId.Value];
            }

            var elder = _elderService.GetElderById(elderId.Value);
            var name = elder?.Fullname ?? "Unknown";
            _elderNames[elderId.Value] = name;
            return name;
        }

        public string GetCaregiverName(int caregiverId)
        {
            if (_caregiverNames.ContainsKey(caregiverId))
            {
                return _caregiverNames[caregiverId];
            }

            var caregiver = _caregiverService.GetCaregiverById(caregiverId);
            string name = "Unknown";

            if (caregiver != null)
            {
                var caregiverAccount = _accountService.GetAccountByIdAsync(caregiver.AccountId).Result;
                if (caregiverAccount != null)
                {
                    name = caregiverAccount.Fullname;
                }
            }

            _caregiverNames[caregiverId] = name;
            return name;
        }

        public string GetCaregiverNameByBookingId(int bookingId)
        {
            if (_bookingToCaregiverId.ContainsKey(bookingId))
            {
                int caregiverId = _bookingToCaregiverId[bookingId];
                return GetCaregiverName(caregiverId);
            }

            // If not in cache, try to get it from the booking
            var booking = _bookingService.GetBookingById(bookingId);
            if (booking != null)
            {
                _bookingToCaregiverId[bookingId] = booking.CaregiverId;
                return GetCaregiverName(booking.CaregiverId);
            }

            return "Unknown";
        }

        public string GetServiceNameByBookingId(int bookingId)
        {
            if (_bookingToServiceName.ContainsKey(bookingId))
            {
                return _bookingToServiceName[bookingId];
            }

            // If not in cache, try to get it from the booking
            var booking = _bookingService.GetBookingById(bookingId);
            if (booking != null && booking.Service != null)
            {
                string serviceName = booking.Service.ServiceName;
                _bookingToServiceName[bookingId] = serviceName;
                return serviceName;
            }

            return "Unknown Service";
        }

        public bool HasFeedback(int bookingId)
        {
            return _bookingsWithFeedback.Contains(bookingId);
        }
    }
}