using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace DataAccessObjects
{
    public class BookingTimeSlotDAO : SingletonBase<BookingTimeSlotDAO>
    {
        // Get all booking time slots
        public List<BookingTimeSlot> GetAllBookingTimeSlots()
        {
            try
            {
                return _context.BookingTimeSlots.Include(bts => bts.Booking).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all booking time slots: " + ex.Message);
            }
        }

        // Get booking time slots by booking ID
        public List<BookingTimeSlot> GetTimeSlotsByBookingId(int bookingId)
        {
            try
            {
                return _context.BookingTimeSlots
                    .Where(bts => bts.BookingId == bookingId)
                    .OrderBy(bts => bts.BookingDate)
                    .ThenBy(bts => bts.StartTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting time slots for booking ID {bookingId}: " + ex.Message);
            }
        }

        // Get a specific booking time slot by ID
        public BookingTimeSlot GetTimeSlotById(int slotId)
        {
            try
            {
                return _context.BookingTimeSlots
                    .Include(bts => bts.Booking)
                    .FirstOrDefault(bts => bts.SlotId == slotId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting time slot with ID {slotId}: " + ex.Message);
            }
        }

        // Add a new booking time slot
        public void AddTimeSlot(BookingTimeSlot timeSlot)
        {
            try
            {
                // Validate the time slot
                if (timeSlot.StartTime >= timeSlot.EndTime)
                {
                    throw new ArgumentException("Start time must be before end time");
                }

                // Check if the booking exists
                var booking = _context.Bookings.Find(timeSlot.BookingId);
                if (booking == null)
                {
                    throw new KeyNotFoundException($"Booking with ID {timeSlot.BookingId} not found");
                }

                // Check if the caregiver is available at this time
                int caregiverId = booking.CaregiverId;
                DateOnly bookingDate = timeSlot.BookingDate;
                TimeOnly startTime = timeSlot.StartTime;
                TimeOnly endTime = timeSlot.EndTime;

                // Get the day of week from the booking date (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)bookingDate.DayOfWeek + 6) % 7 + 1;

                // Check if the caregiver has availability for this day and time
                bool isAvailable = _context.CaregiverAvailabilities
                    .Any(ca => ca.CaregiverId == caregiverId &&
                              ca.DayOfWeek == dayOfWeek &&
                              ca.StartTime <= startTime &&
                              ca.EndTime >= endTime &&
                              ca.IsAvailable == true);

                if (!isAvailable)
                {
                    throw new InvalidOperationException("The caregiver is not available at this time");
                }

                // Check if the caregiver already has a booking at this time
                bool hasOverlappingBooking = _context.Bookings
                    .Join(_context.BookingTimeSlots,
                          b => b.BookingId,
                          bts => bts.BookingId,
                          (b, bts) => new { Booking = b, TimeSlot = bts })
                    .Where(x => x.Booking.CaregiverId == caregiverId &&
                                x.TimeSlot.BookingDate == bookingDate &&
                                ((x.TimeSlot.StartTime <= startTime && x.TimeSlot.EndTime > startTime) ||
                                 (x.TimeSlot.StartTime < endTime && x.TimeSlot.EndTime >= endTime) ||
                                 (x.TimeSlot.StartTime >= startTime && x.TimeSlot.EndTime <= endTime)) &&
                                x.Booking.Status != "rejected" &&
                                x.Booking.Status != "completed" &&
                                x.Booking.Status != "canceled")
                    .Any();

                if (hasOverlappingBooking)
                {
                    throw new InvalidOperationException("The caregiver already has a booking at this time");
                }

                _context.BookingTimeSlots.Add(timeSlot);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding booking time slot: " + ex.Message);
            }
        }

        // Update an existing booking time slot
        public void UpdateTimeSlot(BookingTimeSlot timeSlot)
        {
            try
            {
                // Validate the time slot
                if (timeSlot.StartTime >= timeSlot.EndTime)
                {
                    throw new ArgumentException("Start time must be before end time");
                }

                var existingTimeSlot = _context.BookingTimeSlots.Find(timeSlot.SlotId);
                if (existingTimeSlot == null)
                {
                    throw new KeyNotFoundException($"Time slot with ID {timeSlot.SlotId} not found");
                }

                // Get the booking and caregiver
                var booking = _context.Bookings.Find(existingTimeSlot.BookingId);
                if (booking == null)
                {
                    throw new KeyNotFoundException($"Booking with ID {existingTimeSlot.BookingId} not found");
                }

                int caregiverId = booking.CaregiverId;
                DateOnly bookingDate = timeSlot.BookingDate;
                TimeOnly startTime = timeSlot.StartTime;
                TimeOnly endTime = timeSlot.EndTime;

                // Get the day of week from the booking date (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)bookingDate.DayOfWeek + 6) % 7 + 1;

                // Check if the caregiver has availability for this day and time
                bool isAvailable = _context.CaregiverAvailabilities
                    .Any(ca => ca.CaregiverId == caregiverId &&
                              ca.DayOfWeek == dayOfWeek &&
                              ca.StartTime <= startTime &&
                              ca.EndTime >= endTime &&
                              ca.IsAvailable == true);

                if (!isAvailable)
                {
                    throw new InvalidOperationException("The caregiver is not available at this time");
                }

                // Check if the caregiver already has another booking at this time (excluding this one)
                bool hasOverlappingBooking = _context.Bookings
                    .Join(_context.BookingTimeSlots,
                          b => b.BookingId,
                          bts => bts.BookingId,
                          (b, bts) => new { Booking = b, TimeSlot = bts })
                    .Where(x => x.Booking.CaregiverId == caregiverId &&
                                x.TimeSlot.BookingDate == bookingDate &&
                                x.TimeSlot.SlotId != timeSlot.SlotId &&
                                ((x.TimeSlot.StartTime <= startTime && x.TimeSlot.EndTime > startTime) ||
                                 (x.TimeSlot.StartTime < endTime && x.TimeSlot.EndTime >= endTime) ||
                                 (x.TimeSlot.StartTime >= startTime && x.TimeSlot.EndTime <= endTime)) &&
                                x.Booking.Status != "rejected" &&
                                x.Booking.Status != "completed" &&
                                x.Booking.Status != "canceled")
                    .Any();

                if (hasOverlappingBooking)
                {
                    throw new InvalidOperationException("The caregiver already has a booking at this time");
                }

                _context.Entry(existingTimeSlot).CurrentValues.SetValues(timeSlot);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating booking time slot: " + ex.Message);
            }
        }

        // Delete a booking time slot
        public void DeleteTimeSlot(int slotId)
        {
            try
            {
                var timeSlot = _context.BookingTimeSlots.Find(slotId);
                if (timeSlot == null)
                {
                    throw new KeyNotFoundException($"Time slot with ID {slotId} not found");
                }

                _context.BookingTimeSlots.Remove(timeSlot);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting booking time slot: " + ex.Message);
            }
        }

        // Get time slots by date range
        public List<BookingTimeSlot> GetTimeSlotsByDateRange(DateOnly startDate, DateOnly endDate)
        {
            try
            {
                return _context.BookingTimeSlots
                    .Where(bts => bts.BookingDate >= startDate && bts.BookingDate <= endDate)
                    .OrderBy(bts => bts.BookingDate)
                    .ThenBy(bts => bts.StartTime)
                    .Include(bts => bts.Booking)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting time slots by date range: " + ex.Message);
            }
        }

        // Get time slots for a specific caregiver
        public List<BookingTimeSlot> GetTimeSlotsByCaregiverId(int caregiverId, DateOnly? startDate = null, DateOnly? endDate = null)
        {
            try
            {
                var query = _context.BookingTimeSlots
                    .Include(bts => bts.Booking)
                    .Where(bts => bts.Booking.CaregiverId == caregiverId);

                if (startDate.HasValue)
                {
                    query = query.Where(bts => bts.BookingDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(bts => bts.BookingDate <= endDate.Value);
                }

                return query
                    .OrderBy(bts => bts.BookingDate)
                    .ThenBy(bts => bts.StartTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting time slots for caregiver ID {caregiverId}: " + ex.Message);
            }
        }

        // Check if a time slot is available for a caregiver
        public bool IsTimeSlotAvailable(int caregiverId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            try
            {
                // Validate the time range
                if (startTime >= endTime)
                {
                    throw new ArgumentException("Start time must be before end time");
                }

                // Get the day of week from the booking date (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)bookingDate.DayOfWeek + 6) % 7 + 1;

                // Check if the caregiver has availability for this day and time
                bool hasAvailability = _context.CaregiverAvailabilities
                    .Any(ca => ca.CaregiverId == caregiverId &&
                              ca.DayOfWeek == dayOfWeek &&
                              ca.StartTime <= startTime &&
                              ca.EndTime >= endTime &&
                              ca.IsAvailable == true);

                if (!hasAvailability)
                {
                    return false;
                }

                // Check if the caregiver already has a booking at this time
                bool hasOverlappingBooking = _context.Bookings
                    .Join(_context.BookingTimeSlots,
                          b => b.BookingId,
                          bts => bts.BookingId,
                          (b, bts) => new { Booking = b, TimeSlot = bts })
                    .Where(x => x.Booking.CaregiverId == caregiverId &&
                                x.TimeSlot.BookingDate == bookingDate &&
                                ((x.TimeSlot.StartTime <= startTime && x.TimeSlot.EndTime > startTime) ||
                                 (x.TimeSlot.StartTime < endTime && x.TimeSlot.EndTime >= endTime) ||
                                 (x.TimeSlot.StartTime >= startTime && x.TimeSlot.EndTime <= endTime)) &&
                                x.Booking.Status != "rejected" &&
                                x.Booking.Status != "completed" &&
                                x.Booking.Status != "canceled")
                    .Any();

                return !hasOverlappingBooking;
            }
            catch (Exception ex)
            {
                throw new Exception("Error checking time slot availability: " + ex.Message);
            }
        }
    }
}