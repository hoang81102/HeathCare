using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace DataAccessObjects
{
    public class BookingDAO : SingletonBase<BookingDAO>
    {
        // Get all bookings
        public List<Booking> GetAllBookings()
        {
            try
            {
                return _context.Bookings
                    .Include(b => b.Account)
                    .Include(b => b.Caregiver)
                    .Include(b => b.Elder)
                    .Include(b => b.Service)
                    .Include(b => b.BookingTimeSlots)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings: " + ex.Message);
            }
        }

        // Get booking by ID
        // Update the GetBookingById method to force a fresh database query
        public Booking GetBookingById(int bookingId)
        {
            try
            {
                // Force EF to query the database instead of returning cached entity
                // Use AsNoTracking() to prevent caching, and/or detach any existing entity
                var existingEntity = _context.Bookings.Local.FirstOrDefault(b => b.BookingId == bookingId);
                if (existingEntity != null)
                {
                    _context.Entry(existingEntity).State = EntityState.Detached;
                }

                // Now fetch a fresh copy from the database
                return _context.Bookings
                    .AsNoTracking()  // Prevents tracking
                    .Include(b => b.Account)
                    .Include(b => b.Caregiver)
                    .Include(b => b.Elder)
                    .Include(b => b.Service)
                    .Include(b => b.BookingTimeSlots)
                    .FirstOrDefault(b => b.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking: " + ex.Message);
            }
        }

        // Get bookings by account ID (for customer)
        public List<Booking> GetBookingsByAccountId(int accountId)
        {
            try
            {
                return _context.Bookings
                    .Include(b => b.Account)
                    .Include(b => b.Caregiver)
                    .Include(b => b.Elder)
                    .Include(b => b.Service)
                    .Include(b => b.BookingTimeSlots)
                    .Where(b => b.AccountId == accountId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings for account: " + ex.Message);
            }
        }

        // Get bookings by caregiver ID
        public List<Booking> GetBookingsByCaregiverId(int caregiverId)
        {
            try
            {
                return _context.Bookings
                    .Include(b => b.Account)
                    .Include(b => b.Caregiver)
                    .Include(b => b.Elder)
                    .Include(b => b.Service)
                    .Include(b => b.BookingTimeSlots)
                    .Where(b => b.CaregiverId == caregiverId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings for caregiver: " + ex.Message);
            }
        }

        // Get bookings by elder ID
        public List<Booking> GetBookingsByElderId(int elderId)
        {
            try
            {
                return _context.Bookings
                    .Include(b => b.Account)
                    .Include(b => b.Caregiver)
                    .Include(b => b.Elder)
                    .Include(b => b.Service)
                    .Include(b => b.BookingTimeSlots)
                    .Where(b => b.ElderId == elderId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings for elder: " + ex.Message);
            }
        }

        // Get bookings by status
        public List<Booking> GetBookingsByStatus(string status)
        {
            try
            {
                return _context.Bookings
                    .Include(b => b.Account)
                    .Include(b => b.Caregiver)
                    .Include(b => b.Elder)
                    .Include(b => b.Service)
                    .Include(b => b.BookingTimeSlots)
                    .Where(b => b.Status == status)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bookings with status: " + ex.Message);
            }
        }

        // Get available caregivers for a specific time (using stored procedure)
        public List<CaregiverAvailabilityResult> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                var dateParam = new SqlParameter("@bookingDate", bookingDate.Date);
                var startTimeParam = new SqlParameter("@startTime", startTime);
                var endTimeParam = new SqlParameter("@endTime", endTime);

                var result = _context.Database
                    .SqlQueryRaw<CaregiverAvailabilityResult>(
                        "EXEC GetAvailableCaregivers @bookingDate, @startTime, @endTime",
                        dateParam, startTimeParam, endTimeParam)
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving available caregivers: " + ex.Message);
            }
        }

        // Get caregiver availability for a specific date (using stored procedure)
        public List<TimeSlotResult> GetCaregiverAvailability(int caregiverId, DateTime bookingDate)
        {
            try
            {
                var caregiverIdParam = new SqlParameter("@caregiverId", caregiverId);
                var dateParam = new SqlParameter("@bookingDate", bookingDate.Date);

                var result = _context.Database
                    .SqlQueryRaw<TimeSlotResult>(
                        "EXEC GetCaregiverAvailability @caregiverId, @bookingDate",
                        caregiverIdParam, dateParam)
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving caregiver availability: " + ex.Message);
            }
        }

        // Create a new booking with time slot (using stored procedure)
        // FIXED: Corrected to match the stored procedure parameter count in the database
        // Create a new booking with time slot (using stored procedure)
        // FIXED: Correctly handle the stored procedure result
        public int CreateBookingWithTimeSlot(int accountId, int serviceId, int caregiverId, int elderId,
                                          DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            try
            {
                // Create parameters according to the stored procedure definition
                var accountIdParam = new SqlParameter("@accountId", accountId);
                var serviceIdParam = new SqlParameter("@serviceId", serviceId);
                var caregiverIdParam = new SqlParameter("@caregiverId", caregiverId);
                var elderIdParam = new SqlParameter("@elderId", elderId);
                var dateParam = new SqlParameter("@bookingDate", bookingDate.Date);
                var startTimeParam = new SqlParameter("@startTime", startTime);
                var endTimeParam = new SqlParameter("@endTime", endTime);

                // Create a simple command to execute the stored procedure
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC CreateBookingWithTimeSlot @accountId, @serviceId, @caregiverId, @elderId, @bookingDate, @startTime, @endTime";
                    command.CommandType = System.Data.CommandType.Text;

                    // Add parameters to the command
                    command.Parameters.Add(accountIdParam);
                    command.Parameters.Add(serviceIdParam);
                    command.Parameters.Add(caregiverIdParam);
                    command.Parameters.Add(elderIdParam);
                    command.Parameters.Add(dateParam);
                    command.Parameters.Add(startTimeParam);
                    command.Parameters.Add(endTimeParam);

                    // Ensure the connection is open
                    if (command.Connection.State != System.Data.ConnectionState.Open)
                    {
                        command.Connection.Open();
                    }

                    // Execute the command and read the result
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int bookingId = reader.GetInt32(reader.GetOrdinal("NewBookingId"));
                            return bookingId;
                        }
                        else
                        {
                            throw new Exception("Failed to create booking: No booking ID returned.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating booking: " + ex.Message);
            }
        }

        // Update booking status (accept/reject) (using stored procedure)
        public void UpdateBookingStatus(int bookingId, string newStatus, string rejectionReason = null)
        {
            try
            {
                var bookingIdParam = new SqlParameter("@bookingId", bookingId);
                var statusParam = new SqlParameter("@newStatus", newStatus);
                var reasonParam = new SqlParameter("@rejectionReason", (object)rejectionReason ?? DBNull.Value);

                _context.Database.ExecuteSqlRaw(
                    "EXEC UpdateBookingStatus @bookingId, @newStatus, @rejectionReason",
                    bookingIdParam, statusParam, reasonParam);
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating booking status: " + ex.Message);
            }
        }

        // Add a booking directly (not using stored procedure)
        public void AddBooking(Booking booking)
        {
            try
            {
                _context.Bookings.Add(booking);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding booking: " + ex.Message);
            }
        }

        // Update a booking
        public void UpdateBooking(Booking booking)
        {
            try
            {
                _context.Entry(booking).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating booking: " + ex.Message);
            }
        }

        // Delete a booking
        public void DeleteBooking(int bookingId)
        {
            try
            {
                var booking = _context.Bookings.Find(bookingId);
                if (booking != null)
                {
                    _context.Bookings.Remove(booking);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting booking: " + ex.Message);
            }
        }

        // Get booking details (using the BookingDetails view)
        public List<BookingDetail> GetBookingDetails()
        {
            try
            {
                return _context.BookingDetails.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking details: " + ex.Message);
            }
        }

        // Get booking detail by ID
        public BookingDetail GetBookingDetailById(int bookingId)
        {
            try
            {
                return _context.BookingDetails.FirstOrDefault(bd => bd.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving booking detail: " + ex.Message);
            }
        }
    }

    // Helper class for stored procedure results
    public class CaregiverAvailabilityResult
    {
        public int CaregiverId { get; set; }
        public string Fullname { get; set; }
    }

    // Helper class for time slot results
    public class TimeSlotResult
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}