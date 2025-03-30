using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessObjects;
using Repositories;

namespace Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public List<Booking> GetAllBookings()
        {
            return _bookingRepository.GetAllBookings();
        }

        public Booking GetBookingById(int bookingId)
        {
            return _bookingRepository.GetBookingById(bookingId);
        }

        public List<Booking> GetBookingsByAccountId(int accountId)
        {
            return _bookingRepository.GetBookingsByAccountId(accountId);
        }

        public List<Booking> GetBookingsByCaregiverId(int caregiverId)
        {
            return _bookingRepository.GetBookingsByCaregiverId(caregiverId);
        }

        public List<Booking> GetBookingsByElderId(int elderId)
        {
            return _bookingRepository.GetBookingsByElderId(elderId);
        }

        public List<Booking> GetBookingsByStatus(string status)
        {
            return _bookingRepository.GetBookingsByStatus(status);
        }

        public List<CaregiverAvailabilityResult> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            return _bookingRepository.GetAvailableCaregivers(bookingDate, startTime, endTime);
        }

        public List<TimeSlotResult> GetCaregiverAvailability(int caregiverId, DateTime bookingDate)
        {
            return _bookingRepository.GetCaregiverAvailability(caregiverId, bookingDate);
        }

        public int CreateBooking(int accountId, int serviceId, int caregiverId, int elderId,
                                DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            // Validate the booking before creation
            if (!IsTimeSlotAvailable(caregiverId, bookingDate, startTime, endTime))
            {
                throw new Exception("The selected time slot is not available.");
            }

            return _bookingRepository.CreateBookingWithTimeSlot(
                accountId, serviceId, caregiverId, elderId, bookingDate, startTime, endTime);
        }

        public bool AcceptBooking(int bookingId)
        {
            try
            {
                _bookingRepository.UpdateBookingStatus(bookingId, "Accepted");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool RejectBooking(int bookingId, string rejectionReason)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason))
            {
                throw new ArgumentException("Rejection reason is required.");
            }

            try
            {
                _bookingRepository.UpdateBookingStatus(bookingId, "Rejected", rejectionReason);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CancelBooking(int bookingId)
        {
            try
            {
                _bookingRepository.UpdateBookingStatus(bookingId, "Cancelled");
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateBooking(Booking booking)
        {
            try
            {
                _bookingRepository.UpdateBooking(booking);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteBooking(int bookingId)
        {
            try
            {
                _bookingRepository.DeleteBooking(bookingId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<BookingDetail> GetBookingDetails()
        {
            return _bookingRepository.GetBookingDetails();
        }

        public BookingDetail GetBookingDetailById(int bookingId)
        {
            return _bookingRepository.GetBookingDetailById(bookingId);
        }

        public bool IsTimeSlotAvailable(int caregiverId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            // First, check if caregiver exists in the available caregivers list for this time
            var availableCaregivers = _bookingRepository.GetAvailableCaregivers(bookingDate, startTime, endTime);
            return availableCaregivers.Any(c => c.CaregiverId == caregiverId);
        }

        public List<Booking> GetUpcomingBookingsByAccountId(int accountId)
        {
            // Get all bookings for the account
            var accountBookings = _bookingRepository.GetBookingsByAccountId(accountId);

            // Filter to only include upcoming bookings (status is Accepted and date is in the future)
            return accountBookings
                .Where(b => b.Status == "Accepted" &&
                            b.BookingDateTime > DateTime.Now)
                .OrderBy(b => b.BookingDateTime)
                .ToList();
        }

        public List<Booking> GetUpcomingBookingsByCaregiverId(int caregiverId)
        {
            // Get all bookings for the caregiver
            var caregiverBookings = _bookingRepository.GetBookingsByCaregiverId(caregiverId);

            // Filter to only include upcoming bookings (status is Accepted and date is in the future)
            return caregiverBookings
                .Where(b => b.Status == "Accepted" &&
                            b.BookingDateTime > DateTime.Now)
                .OrderBy(b => b.BookingDateTime)
                .ToList();
        }

        public List<Booking> GetCompletedBookingsByAccountId(int accountId)
        {
            // Get all bookings for the account
            var accountBookings = _bookingRepository.GetBookingsByAccountId(accountId);

            // Filter to only include completed bookings (status is Completed)
            return accountBookings
                .Where(b => b.Status == "Completed")
                .OrderByDescending(b => b.BookingDateTime)
                .ToList();
        }
    }
}