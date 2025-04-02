using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace DataAccessObjects
{
    public class CaregiverDAO : SingletonBase<CaregiverDAO>
    {
        // Get all caregivers with their account information
        public List<Caregiver> GetAllCaregivers()
        {
            try
            {
                return _context.Caregivers
                    .Include(c => c.Account)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving caregivers: " + ex.Message);
            }
        }

        // Get caregiver by ID
        public Caregiver GetCaregiverById(int caregiverId)
        {
            try
            {
                return _context.Caregivers
                    .Include(c => c.Account)
                    .Include(c => c.CaregiverAvailabilities)
                    .FirstOrDefault(c => c.CaregiverId == caregiverId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving caregiver with ID {caregiverId}: " + ex.Message);
            }
        }

        // Get caregiver by account ID
        public Caregiver GetCaregiverByAccountId(int accountId)
        {
            try
            {
                return _context.Caregivers
                    .Include(c => c.Account)
                    .FirstOrDefault(c => c.AccountId == accountId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving caregiver with account ID {accountId}: " + ex.Message);
            }
        }

        // Add new caregiver
        public void AddCaregiver(Caregiver caregiver)
        {
            try
            {
                _context.Caregivers.Add(caregiver);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding caregiver: " + ex.Message);
            }
        }

        // Update caregiver
        public void UpdateCaregiver(Caregiver caregiver)
        {
            try
            {
                var existingCaregiver = _context.Caregivers.Find(caregiver.CaregiverId);
                if (existingCaregiver == null)
                {
                    throw new Exception($"Caregiver with ID {caregiver.CaregiverId} not found");
                }

                _context.Entry(existingCaregiver).CurrentValues.SetValues(caregiver);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating caregiver: " + ex.Message);
            }
        }

        // Delete caregiver
        public void DeleteCaregiver(int caregiverId)
        {
            try
            {
                var caregiver = _context.Caregivers.Find(caregiverId);
                if (caregiver != null)
                {
                    _context.Caregivers.Remove(caregiver);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception($"Caregiver with ID {caregiverId} not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting caregiver: " + ex.Message);
            }
        }

        // Get available caregivers for a specific time
        public List<Caregiver> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                // Get day of week (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)bookingDate.DayOfWeek + 6) % 7 + 1;

                // Use raw SQL to call the stored procedure
                var query = _context.Caregivers
                    .FromSqlRaw("EXEC GetAvailableCaregivers @bookingDate, @startTime, @endTime",
                        new Microsoft.Data.SqlClient.SqlParameter("@bookingDate", bookingDate.Date),
                        new Microsoft.Data.SqlClient.SqlParameter("@startTime", startTime),
                        new Microsoft.Data.SqlClient.SqlParameter("@endTime", endTime))
                    .Include(c => c.Account);

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving available caregivers: " + ex.Message);
            }
        }

        // Add caregiver availability
        public void AddCaregiverAvailability(CaregiverAvailability availability)
        {
            try
            {
                _context.CaregiverAvailabilities.Add(availability);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding caregiver availability: " + ex.Message);
            }
        }

        // Update caregiver availability
        public void UpdateCaregiverAvailability(CaregiverAvailability availability)
        {
            try
            {
                var existingAvailability = _context.CaregiverAvailabilities.Find(availability.AvailabilityId);
                if (existingAvailability == null)
                {
                    throw new Exception($"Availability with ID {availability.AvailabilityId} not found");
                }

                _context.Entry(existingAvailability).CurrentValues.SetValues(availability);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating caregiver availability: " + ex.Message);
            }
        }

        // Delete caregiver availability
        public void DeleteCaregiverAvailability(int availabilityId)
        {
            try
            {
                var availability = _context.CaregiverAvailabilities.Find(availabilityId);
                if (availability != null)
                {
                    _context.CaregiverAvailabilities.Remove(availability);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception($"Availability with ID {availabilityId} not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting caregiver availability: " + ex.Message);
            }
        }

        // Get caregiver availability for a specific date
        public List<CaregiverAvailability> GetCaregiverAvailability(int caregiverId, DateTime bookingDate)
        {
            try
            {
                // Get day of week (1-7 for Monday-Sunday)
                int dayOfWeek = ((int)bookingDate.DayOfWeek + 6) % 7 + 1;

                // Use raw SQL to call the stored procedure
                var query = _context.CaregiverAvailabilities
                    .FromSqlRaw("EXEC GetCaregiverAvailability @caregiverId, @bookingDate",
                        new Microsoft.Data.SqlClient.SqlParameter("@caregiverId", caregiverId),
                        new Microsoft.Data.SqlClient.SqlParameter("@bookingDate", bookingDate.Date));

                return query.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving caregiver availability: " + ex.Message);
            }
        }

        // Get all availability for a caregiver
        public List<CaregiverAvailability> GetAllAvailabilityForCaregiver(int caregiverId)
        {
            try
            {
                return _context.CaregiverAvailabilities
                    .Where(ca => ca.CaregiverId == caregiverId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving availabilities for caregiver ID {caregiverId}: " + ex.Message);
            }
        }

        // Get caregivers by specialty
        public List<Caregiver> GetCaregiversBySpecialty(string specialty)
        {
            try
            {
                return _context.Caregivers
                    .Include(c => c.Account)
                    .Where(c => c.Specialty.Contains(specialty))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving caregivers with specialty {specialty}: " + ex.Message);
            }
        }

        // Get top-rated caregivers
        public List<Caregiver> GetTopRatedCaregivers(int limit = 5)
        {
            try
            {
                // Get caregivers with their average ratings
                var caregiverRatings = from c in _context.Caregivers
                                       join b in _context.Bookings on c.CaregiverId equals b.CaregiverId
                                       join f in _context.Feedbacks on b.BookingId equals f.BookingId
                                       group f by c.CaregiverId into g
                                       select new
                                       {
                                           CaregiverId = g.Key,
                                           AverageRating = g.Average(f => f.Rating)
                                       };

                // Join with caregiver table and sort by rating
                var topCaregivers = from c in _context.Caregivers
                                    join r in caregiverRatings on c.CaregiverId equals r.CaregiverId
                                    orderby r.AverageRating descending
                                    select c;

                return topCaregivers
                    .Include(c => c.Account)
                    .Take(limit)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving top-rated caregivers: " + ex.Message);
            }
        }
    }
}