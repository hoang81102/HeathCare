using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace DataAccessObjects
{
    public class CaregiverAvailabilityDAO : SingletonBase<CaregiverAvailabilityDAO>
    {
        // Get all availabilities
        public List<CaregiverAvailability> GetAllAvailabilities()
        {
            try
            {
                return _context.CaregiverAvailabilities.Include(ca => ca.Caregiver).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all caregiver availabilities: " + ex.Message);
            }
        }

        // Get all availabilities for a specific caregiver
        public List<CaregiverAvailability> GetAvailabilitiesByCaregiverId(int caregiverId)
        {
            try
            {
                return _context.CaregiverAvailabilities
                    .Where(ca => ca.CaregiverId == caregiverId)
                    .OrderBy(ca => ca.DayOfWeek)
                    .ThenBy(ca => ca.StartTime)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting availabilities for caregiver ID {caregiverId}: " + ex.Message);
            }
        }

        // Get a specific availability by ID
        public CaregiverAvailability GetAvailabilityById(int availabilityId)
        {
            try
            {
                return _context.CaregiverAvailabilities
                    .Include(ca => ca.Caregiver)
                    .FirstOrDefault(ca => ca.AvailabilityId == availabilityId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting availability with ID {availabilityId}: " + ex.Message);
            }
        }

        // Add a new availability
        public void AddAvailability(CaregiverAvailability availability)
        {
            try
            {
                // Validate the availability time range
                if (availability.StartTime >= availability.EndTime)
                {
                    throw new ArgumentException("Start time must be before end time");
                }

                // Check for overlapping availabilities for this caregiver on the same day
                bool hasOverlap = _context.CaregiverAvailabilities
                    .Where(ca => ca.CaregiverId == availability.CaregiverId &&
                                 ca.DayOfWeek == availability.DayOfWeek)
                    .Any(ca => (availability.StartTime < ca.EndTime &&
                               availability.EndTime > ca.StartTime));

                if (hasOverlap)
                {
                    throw new InvalidOperationException("The new availability overlaps with an existing availability");
                }

                _context.CaregiverAvailabilities.Add(availability);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding caregiver availability: " + ex.Message);
            }
        }

        // Update an existing availability
        public void UpdateAvailability(CaregiverAvailability availability)
        {
            try
            {
                // Validate the availability time range
                if (availability.StartTime >= availability.EndTime)
                {
                    throw new ArgumentException("Start time must be before end time");
                }

                // Check for overlapping availabilities (excluding the current one)
                bool hasOverlap = _context.CaregiverAvailabilities
                    .Where(ca => ca.CaregiverId == availability.CaregiverId &&
                                 ca.DayOfWeek == availability.DayOfWeek &&
                                 ca.AvailabilityId != availability.AvailabilityId)
                    .Any(ca => (availability.StartTime < ca.EndTime &&
                               availability.EndTime > ca.StartTime));

                if (hasOverlap)
                {
                    throw new InvalidOperationException("The updated availability overlaps with an existing availability");
                }

                var existingAvailability = _context.CaregiverAvailabilities.Find(availability.AvailabilityId);
                if (existingAvailability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {availability.AvailabilityId} not found");
                }

                _context.Entry(existingAvailability).CurrentValues.SetValues(availability);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating caregiver availability: " + ex.Message);
            }
        }

        // Delete an availability
        public void DeleteAvailability(int availabilityId)
        {
            try
            {
                var availability = _context.CaregiverAvailabilities.Find(availabilityId);
                if (availability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {availabilityId} not found");
                }

                _context.CaregiverAvailabilities.Remove(availability);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting caregiver availability: " + ex.Message);
            }
        }

        // Get available caregivers for a specific time slot
        public List<Caregiver> GetAvailableCaregivers(DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            try
            {
                // Get the day of week from the booking date (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)bookingDate.DayOfWeek + 6) % 7 + 1;

                // Find caregivers who have availability for the specified day and time
                var availableCaregiverIds = _context.CaregiverAvailabilities
                    .Where(ca => ca.DayOfWeek == dayOfWeek &&
                                 ca.StartTime <= startTime &&
                                 ca.EndTime >= endTime &&
                                 ca.IsAvailable == true)
                    .Select(ca => ca.CaregiverId)
                    .Distinct()
                    .ToList();

                // Exclude caregivers who have overlapping bookings
                var busyCaregiverIds = _context.Bookings
                    .Join(_context.BookingTimeSlots,
                          b => b.BookingId,
                          bts => bts.BookingId,
                          (b, bts) => new { Booking = b, TimeSlot = bts })
                    .Where(x => x.TimeSlot.BookingDate == bookingDate &&
                               ((x.TimeSlot.StartTime <= startTime && x.TimeSlot.EndTime > startTime) ||
                                (x.TimeSlot.StartTime < endTime && x.TimeSlot.EndTime >= endTime) ||
                                (x.TimeSlot.StartTime >= startTime && x.TimeSlot.EndTime <= endTime)) &&
                               x.Booking.Status != "rejected" &&
                               x.Booking.Status != "completed" &&
                               x.Booking.Status != "canceled")
                    .Select(x => x.Booking.CaregiverId)
                    .Distinct()
                    .ToList();

                // Get the caregivers who are available
                return _context.Caregivers
                    .Include(c => c.Account)
                    .Where(c => availableCaregiverIds.Contains(c.CaregiverId) &&
                               !busyCaregiverIds.Contains(c.CaregiverId) &&
                               c.Account.AccountStatus == "active")
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error finding available caregivers: " + ex.Message);
            }
        }

        // Get available time slots for a specific caregiver on a specific date
        public List<(TimeOnly StartTime, TimeOnly EndTime)> GetAvailableTimeSlots(int caregiverId, DateOnly bookingDate)
        {
            try
            {
                // Get the day of week from the booking date (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)bookingDate.DayOfWeek + 6) % 7 + 1;

                // Get the caregiver's availability for that day
                var availabilitySlots = _context.CaregiverAvailabilities
                    .Where(ca => ca.CaregiverId == caregiverId &&
                                 ca.DayOfWeek == dayOfWeek &&
                                 ca.IsAvailable == true)
                    .Select(ca => new { ca.StartTime, ca.EndTime })
                    .ToList();

                if (!availabilitySlots.Any())
                {
                    return new List<(TimeOnly, TimeOnly)>();
                }

                // Get the caregiver's existing bookings for that date
                var existingBookings = _context.Bookings
                    .Join(_context.BookingTimeSlots,
                          b => b.BookingId,
                          bts => bts.BookingId,
                          (b, bts) => new { Booking = b, TimeSlot = bts })
                    .Where(x => x.Booking.CaregiverId == caregiverId &&
                               x.TimeSlot.BookingDate == bookingDate &&
                               x.Booking.Status != "rejected" &&
                               x.Booking.Status != "completed" &&
                               x.Booking.Status != "canceled")
                    .Select(x => new { x.TimeSlot.StartTime, x.TimeSlot.EndTime })
                    .ToList();

                // If there are no existing bookings, return all availability slots
                if (!existingBookings.Any())
                {
                    return availabilitySlots.Select(a => (a.StartTime, a.EndTime)).ToList();
                }

                // Create a list to store available time slots
                var availableTimeSlots = new List<(TimeOnly StartTime, TimeOnly EndTime)>();

                // For each availability slot, determine available time slots
                foreach (var slot in availabilitySlots)
                {
                    // Add the full slot if no bookings overlap with it
                    if (!existingBookings.Any(b =>
                          (b.StartTime <= slot.StartTime && b.EndTime > slot.StartTime) ||
                          (b.StartTime < slot.EndTime && b.EndTime >= slot.EndTime) ||
                          (b.StartTime >= slot.StartTime && b.EndTime <= slot.EndTime)))
                    {
                        availableTimeSlots.Add((slot.StartTime, slot.EndTime));
                    }
                    else
                    {
                        // This is a simplified approach for time slot calculation
                        // Create a list of all time points (start and end of all bookings)
                        var timePoints = new List<TimeOnly>();
                        timePoints.Add(slot.StartTime);
                        timePoints.Add(slot.EndTime);

                        foreach (var booking in existingBookings)
                        {
                            if (booking.StartTime >= slot.StartTime && booking.StartTime < slot.EndTime)
                            {
                                timePoints.Add(booking.StartTime);
                            }
                            if (booking.EndTime > slot.StartTime && booking.EndTime <= slot.EndTime)
                            {
                                timePoints.Add(booking.EndTime);
                            }
                        }

                        // Sort the time points
                        timePoints = timePoints.OrderBy(t => t).ToList();

                        // Check each potential time slot
                        for (int i = 0; i < timePoints.Count - 1; i++)
                        {
                            var potentialStart = timePoints[i];
                            var potentialEnd = timePoints[i + 1];

                            // Skip if this is a zero-length slot
                            if (potentialStart == potentialEnd) continue;

                            // Check if this slot is free
                            if (!existingBookings.Any(b =>
                                (b.StartTime <= potentialStart && b.EndTime > potentialStart) ||
                                (b.StartTime < potentialEnd && b.EndTime >= potentialEnd) ||
                                (b.StartTime >= potentialStart && b.EndTime <= potentialEnd)))
                            {
                                availableTimeSlots.Add((potentialStart, potentialEnd));
                            }
                        }
                    }
                }

                return availableTimeSlots;
            }
            catch (Exception ex)
            {
                throw new Exception("Error finding available time slots: " + ex.Message);
            }
        }

        // Toggle availability status
        public void ToggleAvailabilityStatus(int availabilityId, bool isAvailable)
        {
            try
            {
                var availability = _context.CaregiverAvailabilities.Find(availabilityId);
                if (availability == null)
                {
                    throw new KeyNotFoundException($"Availability with ID {availabilityId} not found");
                }

                availability.IsAvailable = isAvailable;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error toggling availability status: " + ex.Message);
            }
        }
    }
}