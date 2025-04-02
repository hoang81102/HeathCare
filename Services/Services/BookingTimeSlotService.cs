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

        
    }
}