﻿using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IBookingTimeSlotService
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

        // Additional business logic methods can be added here
        TimeSpan CalculateTotalDuration(int bookingId);
        List<BookingTimeSlot> GetUpcomingTimeSlots(int caregiverId, int daysAhead = 7);
        List<DateOnly> GetAvailableDates(int caregiverId, DateOnly startDate, DateOnly endDate);
    }
}