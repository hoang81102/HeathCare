using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class BookingTimeSlotService : IBookingTimeSlotService
    {
        private readonly IBookingTimeSlotRepository _bookingTimeSlotRepository;

        public BookingTimeSlotService(IBookingTimeSlotRepository bookingTimeSlotRepository)
        {
            _bookingTimeSlotRepository = bookingTimeSlotRepository;
        }

        public List<BookingTimeSlot> GetAllBookingTimeSlots()
        {
            return _bookingTimeSlotRepository.GetAllBookingTimeSlots();
        }

        public List<BookingTimeSlot> GetTimeSlotsByBookingId(int bookingId)
        {
            return _bookingTimeSlotRepository.GetTimeSlotsByBookingId(bookingId);
        }

        public BookingTimeSlot GetTimeSlotById(int slotId)
        {
            return _bookingTimeSlotRepository.GetTimeSlotById(slotId);
        }

        public void AddTimeSlot(BookingTimeSlot timeSlot)
        {
            if (timeSlot == null)
            {
                throw new ArgumentNullException(nameof(timeSlot), "Time slot cannot be null");
            }

            if (timeSlot.BookingDate < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException("Booking date cannot be in the past");
            }

            _bookingTimeSlotRepository.AddTimeSlot(timeSlot);
        }

        public void UpdateTimeSlot(BookingTimeSlot timeSlot)
        {
            if (timeSlot == null)
            {
                throw new ArgumentNullException(nameof(timeSlot), "Time slot cannot be null");
            }

            if (timeSlot.BookingDate < DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentException("Booking date cannot be in the past");
            }

            _bookingTimeSlotRepository.UpdateTimeSlot(timeSlot);
        }

        public void DeleteTimeSlot(int slotId)
        {
            var timeSlot = _bookingTimeSlotRepository.GetTimeSlotById(slotId);
            if (timeSlot == null)
            {
                throw new KeyNotFoundException($"Time slot with ID {slotId} not found");
            }

            _bookingTimeSlotRepository.DeleteTimeSlot(slotId);
        }

        public List<BookingTimeSlot> GetTimeSlotsByDateRange(DateOnly startDate, DateOnly endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before or equal to end date");
            }

            return _bookingTimeSlotRepository.GetTimeSlotsByDateRange(startDate, endDate);
        }

        public List<BookingTimeSlot> GetTimeSlotsByCaregiverId(int caregiverId, DateOnly? startDate = null, DateOnly? endDate = null)
        {
            if (startDate.HasValue && endDate.HasValue && startDate.Value > endDate.Value)
            {
                throw new ArgumentException("Start date must be before or equal to end date");
            }

            return _bookingTimeSlotRepository.GetTimeSlotsByCaregiverId(caregiverId, startDate, endDate);
        }

        public bool IsTimeSlotAvailable(int caregiverId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            if (startTime >= endTime)
            {
                throw new ArgumentException("Start time must be before end time");
            }

            return _bookingTimeSlotRepository.IsTimeSlotAvailable(caregiverId, bookingDate, startTime, endTime);
        }

        // Additional business logic methods
        public TimeSpan CalculateTotalDuration(int bookingId)
        {
            var timeSlots = _bookingTimeSlotRepository.GetTimeSlotsByBookingId(bookingId);
            TimeSpan totalDuration = TimeSpan.Zero;

            foreach (var slot in timeSlots)
            {
                var duration = slot.EndTime.ToTimeSpan() - slot.StartTime.ToTimeSpan();
                totalDuration += duration;
            }

            return totalDuration;
        }

        public List<BookingTimeSlot> GetUpcomingTimeSlots(int caregiverId, int daysAhead = 7)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var endDate = today.AddDays(daysAhead);

            return _bookingTimeSlotRepository.GetTimeSlotsByCaregiverId(caregiverId, today, endDate)
                .OrderBy(ts => ts.BookingDate)
                .ThenBy(ts => ts.StartTime)
                .ToList();
        }

        public List<DateOnly> GetAvailableDates(int caregiverId, DateOnly startDate, DateOnly endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("Start date must be before or equal to end date");
            }

            var availableDates = new List<DateOnly>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                // Get the day of week (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)currentDate.DayOfWeek + 6) % 7 + 1;

                // Check if caregiver is available on this day of week
                // This assumes we have access to context, but we don't through repository
                // So this is a simplified version - in a real implementation we might want to add a repository method
                // for this specific check or refactor to get caregiver availabilities

                // For now, we'll simulate by checking if there are any bookings for this specific date
                bool hasBookings = _bookingTimeSlotRepository.GetTimeSlotsByCaregiverId(caregiverId, currentDate, currentDate).Any();

                // If no bookings on this date, and it's in the future, add it to available dates
                if (!hasBookings && currentDate >= DateOnly.FromDateTime(DateTime.Today))
                {
                    availableDates.Add(currentDate);
                }

                currentDate = currentDate.AddDays(1);
            }

            return availableDates;
        }
    }
}