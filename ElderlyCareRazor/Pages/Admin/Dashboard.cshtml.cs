using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IBookingService _bookingService;
        private readonly ICaregiverService _caregiverService;
        private readonly IElderService _elderService;
        private readonly IFeedbackService _feedbackService;

        public DashboardModel(
            IAccountService accountService,
            IServiceService serviceService,
            IServiceCategoryService serviceCategoryService,
            IBookingService bookingService,
            ICaregiverService caregiverService,
            IElderService elderService,
            IFeedbackService feedbackService)
        {
            _accountService = accountService;
            _serviceService = serviceService;
            _serviceCategoryService = serviceCategoryService;
            _bookingService = bookingService;
            _caregiverService = caregiverService;
            _elderService = elderService;
            _feedbackService = feedbackService;
        }

        // Dashboard stats
        public int TotalAccounts { get; set; }
        public int TotalCaregivers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalElders { get; set; }

        // Booking stats
        public int PendingBookings { get; set; }
        public int AcceptedBookings { get; set; }
        public int InProgressBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int RejectedBookings { get; set; }
        public int CancelledBookings { get; set; }

        // Service categories
        public List<ServiceCategoryViewModel> ServiceCategories { get; set; } = new List<ServiceCategoryViewModel>();

        // Recent bookings
        public List<BookingViewModel> RecentBookings { get; set; } = new List<BookingViewModel>();

        // Top caregivers
        public List<CaregiverViewModel> TopCaregivers { get; set; } = new List<CaregiverViewModel>();

        public class ServiceCategoryViewModel
        {
            public string CategoryName { get; set; }
            public int ServiceCount { get; set; }
            public int BookingCount { get; set; }
        }

        public class BookingViewModel
        {
            public int BookingId { get; set; }
            public string CustomerName { get; set; }
            public string ElderName { get; set; }
            public string ServiceName { get; set; }
            public string Status { get; set; }
            public DateTime BookingDate { get; set; }
        }

        public class CaregiverViewModel
        {
            public string FullName { get; set; }
            public int ExperienceYears { get; set; }
            public string Specialty { get; set; }
            public double Rating { get; set; }
        }

        public async Task OnGetAsync()
        {
            // Get all accounts data
            var accounts = await _accountService.GetAllAccountsAsync();
            TotalAccounts = accounts.Count;
            TotalCustomers = accounts.Count(a => a.RoleId == 2); // Customer role

            // Get caregivers data
            var caregivers = _caregiverService.GetAllCaregivers();
            TotalCaregivers = caregivers.Count;

            // Get elders data
            var elders = _elderService.GetAllElders();
            TotalElders = elders.Count;

            // Get booking stats
            var allBookings = _bookingService.GetAllBookings();
            PendingBookings = allBookings.Count(b => b.Status.ToLower() == "pending");
            AcceptedBookings = allBookings.Count(b => b.Status.ToLower() == "accepted");
            InProgressBookings = allBookings.Count(b => b.Status.ToLower() == "in-progress");
            CompletedBookings = allBookings.Count(b => b.Status.ToLower() == "completed");
            RejectedBookings = allBookings.Count(b => b.Status.ToLower() == "rejected");
            CancelledBookings = allBookings.Count(b => b.Status.ToLower() == "canceled");

            // Get service categories with counts
            var categoriesWithCounts = _serviceCategoryService.GetCategoriesWithServiceCount();
            foreach (var (category, serviceCount) in categoriesWithCounts)
            {
                var bookingCount = allBookings.Count(b => {
                    var service = _serviceService.GetServiceById(b.ServiceId);
                    return service != null && service.CategoryId == category.CategoryId;
                });

                ServiceCategories.Add(new ServiceCategoryViewModel
                {
                    CategoryName = category.CategoryName,
                    ServiceCount = serviceCount,
                    BookingCount = bookingCount
                });
            }

            // Get recent bookings
            var bookingDetails = _bookingService.GetBookingDetails()
                .OrderByDescending(b => b.BookingDate)
                .Take(10)
                .ToList();

            foreach (var booking in bookingDetails)
            {
                RecentBookings.Add(new BookingViewModel
                {
                    BookingId = booking.BookingId,
                    CustomerName = booking.CustomerName,
                    ElderName = booking.ElderName,
                    ServiceName = booking.ServiceName,
                    Status = booking.Status,
                    BookingDate = DateTime.Parse(booking.BookingDate.ToString())
                });
            }

            // Get top rated caregivers
            var topCaregivers = _caregiverService.GetTopRatedCaregivers(5);
            foreach (var caregiver in topCaregivers)
            {
                var account = await _accountService.GetAccountByIdAsync(caregiver.AccountId);
                var rating = _feedbackService.GetAverageRatingForCaregiver(caregiver.CaregiverId);

                TopCaregivers.Add(new CaregiverViewModel
                {
                    FullName = account?.Fullname ?? "Unknown",
                    ExperienceYears = caregiver.ExperienceYears,
                    Specialty = caregiver.Specialty ?? "General Care",
                    Rating = rating
                });
            }
        }
    }
}