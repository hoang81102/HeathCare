using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessObjects;

namespace Services
{
    public interface IBookingService
    {
        // Get operations
        List<Booking> GetAllBookings();
        Booking GetBookingById(int bookingId);
        List<Booking> GetBookingsByAccountId(int accountId);
        List<Booking> GetBookingsByCaregiverId(int caregiverId);
        List<Booking> GetBookingsByElderId(int elderId);
        List<Booking> GetBookingsByStatus(string status);

        // Availability operations
        List<CaregiverAvailabilityResult> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
        List<TimeSlotResult> GetCaregiverAvailability(int caregiverId, DateTime bookingDate);

        // Booking operations
        int CreateBooking(int accountId, int serviceId, int caregiverId, int elderId,
                         DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
        bool AcceptBooking(int bookingId);
        bool RejectBooking(int bookingId, string rejectionReason);
        bool CancelBooking(int bookingId);
        bool UpdateBooking(Booking booking);
        bool DeleteBooking(int bookingId);

        // Booking details
        List<BookingDetail> GetBookingDetails();
        BookingDetail GetBookingDetailById(int bookingId);

        // Business logic methods
        bool IsTimeSlotAvailable(int caregiverId, DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
        List<Booking> GetUpcomingBookingsByAccountId(int accountId);
        List<Booking> GetUpcomingBookingsByCaregiverId(int caregiverId);
        List<Booking> GetCompletedBookingsByAccountId(int accountId);
    }
}