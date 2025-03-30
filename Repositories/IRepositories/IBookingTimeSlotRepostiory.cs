using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface IBookingTimeSlotRepository
    {
        List<BookingTimeSlot> GetAllBookingTimeSlots();
        List<BookingTimeSlot> GetTimeSlotsByBookingId(int bookingId);
        BookingTimeSlot GetTimeSlotById(int slotId);
        void AddTimeSlot(BookingTimeSlot timeSlot);
        void UpdateTimeSlot(BookingTimeSlot timeSlot);
        void DeleteTimeSlot(int slotId);
        List<BookingTimeSlot> GetTimeSlotsByDateRange(DateOnly startDate, DateOnly endDate);
        List<BookingTimeSlot> GetTimeSlotsByCaregiverId(int caregiverId, DateOnly? startDate = null, DateOnly? endDate = null);
        bool IsTimeSlotAvailable(int caregiverId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime);
    }
}