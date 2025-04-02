using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public interface IBookingRepository
    {
        List<Booking> GetAllBookings();

        Booking GetBookingById(int bookingId);

        List<Booking> GetBookingsByAccountId(int accountId);

        List<Booking> GetBookingsByCaregiverId(int caregiverId);

        List<Booking> GetBookingsByElderId(int elderId);

        List<Booking> GetBookingsByStatus(string status);

        List<CaregiverAvailabilityResult> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);

        List<TimeSlotResult> GetCaregiverAvailability(int caregiverId, DateTime bookingDate);

        int CreateBookingWithTimeSlot(int accountId, int serviceId, int caregiverId, int elderId,
                                   DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);

        void UpdateBookingStatus(int bookingId, string newStatus, string rejectionReason = null);

        void AddBooking(Booking booking);

        void UpdateBooking(Booking booking);

        void DeleteBooking(int bookingId);

        List<BookingDetail> GetBookingDetails();

        BookingDetail GetBookingDetailById(int bookingId);
    }
}