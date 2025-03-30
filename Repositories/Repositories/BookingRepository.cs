using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDAO _bookingDAO;

        public BookingRepository()
        {
            _bookingDAO = BookingDAO.Instance;
        }

        public List<Booking> GetAllBookings()
        {
            return _bookingDAO.GetAllBookings();
        }

        public Booking GetBookingById(int bookingId)
        {
            return _bookingDAO.GetBookingById(bookingId);
        }

        public List<Booking> GetBookingsByAccountId(int accountId)
        {
            return _bookingDAO.GetBookingsByAccountId(accountId);
        }

        public List<Booking> GetBookingsByCaregiverId(int caregiverId)
        {
            return _bookingDAO.GetBookingsByCaregiverId(caregiverId);
        }

        public List<Booking> GetBookingsByElderId(int elderId)
        {
            return _bookingDAO.GetBookingsByElderId(elderId);
        }

        public List<Booking> GetBookingsByStatus(string status)
        {
            return _bookingDAO.GetBookingsByStatus(status);
        }

        public List<CaregiverAvailabilityResult> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            return _bookingDAO.GetAvailableCaregivers(bookingDate, startTime, endTime);
        }

        public List<TimeSlotResult> GetCaregiverAvailability(int caregiverId, DateTime bookingDate)
        {
            return _bookingDAO.GetCaregiverAvailability(caregiverId, bookingDate);
        }

        public int CreateBookingWithTimeSlot(int accountId, int serviceId, int caregiverId, int elderId,
                                           DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            return _bookingDAO.CreateBookingWithTimeSlot(accountId, serviceId, caregiverId, elderId,
                                                       bookingDate, startTime, endTime);
        }

        public void UpdateBookingStatus(int bookingId, string newStatus, string rejectionReason = null)
        {
            _bookingDAO.UpdateBookingStatus(bookingId, newStatus, rejectionReason);
        }

        public void AddBooking(Booking booking)
        {
            _bookingDAO.AddBooking(booking);
        }

        public void UpdateBooking(Booking booking)
        {
            _bookingDAO.UpdateBooking(booking);
        }

        public void DeleteBooking(int bookingId)
        {
            _bookingDAO.DeleteBooking(bookingId);
        }

        public List<BookingDetail> GetBookingDetails()
        {
            return _bookingDAO.GetBookingDetails();
        }

        public BookingDetail GetBookingDetailById(int bookingId)
        {
            return _bookingDAO.GetBookingDetailById(bookingId);
        }
    }
}