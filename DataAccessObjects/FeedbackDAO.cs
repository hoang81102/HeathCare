using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace DataAccessObjects
{
    public class FeedbackDAO : SingletonBase<FeedbackDAO>
    {
        // Get all feedbacks
        public List<Feedback> GetAllFeedbacks()
        {
            try
            {
                return _context.Feedbacks
                    .Include(f => f.Booking)
                    .ThenInclude(b => b.Caregiver)
                    .ThenInclude(c => c.Account)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving feedbacks: " + ex.Message);
            }
        }

        // Get feedback by ID
        public Feedback GetFeedbackById(int id)
        {
            try
            {
                return _context.Feedbacks
                    .Include(f => f.Booking)
                    .ThenInclude(b => b.Caregiver)
                    .ThenInclude(c => c.Account)
                    .FirstOrDefault(f => f.FeedbackId == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving feedback with ID {id}: " + ex.Message);
            }
        }

        // Get feedbacks by booking ID
        public List<Feedback> GetFeedbacksByBookingId(int bookingId)
        {
            try
            {
                return _context.Feedbacks
                    .Where(f => f.BookingId == bookingId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving feedbacks for booking ID {bookingId}: " + ex.Message);
            }
        }

        // Get feedbacks by caregiver ID
        public List<Feedback> GetFeedbacksByCaregiverId(int caregiverId)
        {
            try
            {
                return _context.Feedbacks
                    .Include(f => f.Booking)
                    .Where(f => f.Booking.CaregiverId == caregiverId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving feedbacks for caregiver ID {caregiverId}: " + ex.Message);
            }
        }

        // Get feedbacks by customer account ID
        public List<Feedback> GetFeedbacksByCustomerId(int accountId)
        {
            try
            {
                return _context.Feedbacks
                    .Include(f => f.Booking)
                    .Where(f => f.Booking.AccountId == accountId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving feedbacks for customer ID {accountId}: " + ex.Message);
            }
        }

        // Add a new feedback
        public void AddFeedback(Feedback feedback)
        {
            try
            {
                // Check if booking exists
                var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == feedback.BookingId);
                if (booking == null)
                {
                    throw new Exception($"Cannot add feedback: Booking with ID {feedback.BookingId} does not exist.");
                }

                // Check if booking is completed
                if (booking.Status != "completed")
                {
                    throw new Exception("Cannot add feedback: Booking is not completed yet.");
                }

                // Check if feedback already exists for this booking
                var existingFeedback = _context.Feedbacks.FirstOrDefault(f => f.BookingId == feedback.BookingId);
                if (existingFeedback != null)
                {
                    throw new Exception($"Feedback already exists for booking ID {feedback.BookingId}.");
                }

                // Set feedback date to current date if not provided
                if (feedback.FeedbackDate == null)
                {
                    feedback.FeedbackDate = DateTime.Now;
                }

                _context.Feedbacks.Add(feedback);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding feedback: " + ex.Message);
            }
        }

        // Update an existing feedback
        public void UpdateFeedback(Feedback feedback)
        {
            try
            {
                var existingFeedback = _context.Feedbacks.FirstOrDefault(f => f.FeedbackId == feedback.FeedbackId);
                if (existingFeedback == null)
                {
                    throw new Exception($"Feedback with ID {feedback.FeedbackId} does not exist.");
                }

                // Update properties
                existingFeedback.Note = feedback.Note;
                existingFeedback.Rating = feedback.Rating;
                existingFeedback.CaregiverProfessionalism = feedback.CaregiverProfessionalism;
                existingFeedback.ServiceQuality = feedback.ServiceQuality;
                existingFeedback.OverallExperience = feedback.OverallExperience;
                existingFeedback.FeedbackDate = DateTime.Now; // Update feedback date on edit

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating feedback: " + ex.Message);
            }
        }

        // Delete a feedback
        public void DeleteFeedback(int id)
        {
            try
            {
                var feedback = _context.Feedbacks.FirstOrDefault(f => f.FeedbackId == id);
                if (feedback == null)
                {
                    throw new Exception($"Feedback with ID {id} does not exist.");
                }

                _context.Feedbacks.Remove(feedback);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting feedback: " + ex.Message);
            }
        }

        // Get average rating for a caregiver
        public double GetAverageRatingForCaregiver(int caregiverId)
        {
            try
            {
                var feedbacks = _context.Feedbacks
                    .Include(f => f.Booking)
                    .Where(f => f.Booking.CaregiverId == caregiverId && f.Rating.HasValue)
                    .ToList();

                if (feedbacks.Count == 0)
                {
                    return 0; // No ratings yet
                }

                return feedbacks.Average(f => f.Rating.Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating average rating for caregiver ID {caregiverId}: " + ex.Message);
            }
        }

        // Get detailed ratings for a caregiver (professionalism, quality, overall)
        public Dictionary<string, double> GetDetailedRatingsForCaregiver(int caregiverId)
        {
            try
            {
                var feedbacks = _context.Feedbacks
                    .Include(f => f.Booking)
                    .Where(f => f.Booking.CaregiverId == caregiverId)
                    .ToList();

                var result = new Dictionary<string, double>
                {
                    { "Professionalism", 0 },
                    { "ServiceQuality", 0 },
                    { "OverallExperience", 0 },
                    { "Average", 0 }
                };

                if (feedbacks.Count == 0)
                {
                    return result; // No ratings yet
                }

                // Calculate averages
                result["Professionalism"] = feedbacks
                    .Where(f => f.CaregiverProfessionalism.HasValue)
                    .Average(f => f.CaregiverProfessionalism.Value);

                result["ServiceQuality"] = feedbacks
                    .Where(f => f.ServiceQuality.HasValue)
                    .Average(f => f.ServiceQuality.Value);

                result["OverallExperience"] = feedbacks
                    .Where(f => f.OverallExperience.HasValue)
                    .Average(f => f.OverallExperience.Value);

                result["Average"] = feedbacks
                    .Where(f => f.Rating.HasValue)
                    .Average(f => f.Rating.Value);

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating detailed ratings for caregiver ID {caregiverId}: " + ex.Message);
            }
        }

        // Add feedback using stored procedure
        public void AddFeedbackWithStoredProcedure(int bookingId, string note, int rating,
            int caregiverProfessionalism, int serviceQuality, int overallExperience)
        {
            try
            {
                // Using raw SQL to call stored procedure
                _context.Database.ExecuteSqlRaw(
                    "EXEC AddFeedback @bookingId, @note, @rating, @caregiverProfessionalism, @serviceQuality, @overallExperience",
                    new Microsoft.Data.SqlClient.SqlParameter("@bookingId", bookingId),
                    new Microsoft.Data.SqlClient.SqlParameter("@note", (object)note ?? DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@rating", rating),
                    new Microsoft.Data.SqlClient.SqlParameter("@caregiverProfessionalism", caregiverProfessionalism),
                    new Microsoft.Data.SqlClient.SqlParameter("@serviceQuality", serviceQuality),
                    new Microsoft.Data.SqlClient.SqlParameter("@overallExperience", overallExperience));
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding feedback using stored procedure: " + ex.Message);
            }
        }
    }
}