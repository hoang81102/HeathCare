using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class BookingTimeSlotRepository : IBookingTimeSlotRepository
    {
        private readonly BookingTimeSlotDAO _bookingTimeSlotDAO;

        public BookingTimeSlotRepository()
        {
            _bookingTimeSlotDAO = BookingTimeSlotDAO.Instance;
        }

        public List<BookingTimeSlot> GetAllBookingTimeSlots()
        {
            return _bookingTimeSlotDAO.GetAllBookingTimeSlots();
        }

        public List<BookingTimeSlot> GetTimeSlotsByBookingId(int bookingId)
        {
            return _bookingTimeSlotDAO.GetTimeSlotsByBookingId(bookingId);
        }

        public BookingTimeSlot GetTimeSlotById(int slotId)
        {
            return _bookingTimeSlotDAO.GetTimeSlotById(slotId);
        }

        public void AddTimeSlot(BookingTimeSlot timeSlot)
        {
            _bookingTimeSlotDAO.AddTimeSlot(timeSlot);
        }

        public void UpdateTimeSlot(BookingTimeSlot timeSlot)
        {
            _bookingTimeSlotDAO.UpdateTimeSlot(timeSlot);
        }

        public void DeleteTimeSlot(int slotId)
        {
            _bookingTimeSlotDAO.DeleteTimeSlot(slotId);
        }

        public List<BookingTimeSlot> GetTimeSlotsByDateRange(DateOnly startDate, DateOnly endDate)
        {
            return _bookingTimeSlotDAO.GetTimeSlotsByDateRange(startDate, endDate);
        }

        public List<BookingTimeSlot> GetTimeSlotsByCaregiverId(int caregiverId, DateOnly? startDate = null, DateOnly? endDate = null)
        {
            return _bookingTimeSlotDAO.GetTimeSlotsByCaregiverId(caregiverId, startDate, endDate);
        }

        public bool IsTimeSlotAvailable(int caregiverId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            return _bookingTimeSlotDAO.IsTimeSlotAvailable(caregiverId, bookingDate, startTime, endTime);
        }
    }
}