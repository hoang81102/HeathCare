using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IBookingRepository _bookingRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository, IBookingRepository bookingRepository)
        {
            _feedbackRepository = feedbackRepository;
            _bookingRepository = bookingRepository;
        }

        public List<Feedback> GetAllFeedbacks()
        {
            return _feedbackRepository.GetAllFeedbacks();
        }

        public Feedback GetFeedbackById(int id)
        {
            return _feedbackRepository.GetFeedbackById(id);
        }

        public List<Feedback> GetFeedbacksByBookingId(int bookingId)
        {
            return _feedbackRepository.GetFeedbacksByBookingId(bookingId);
        }

        public List<Feedback> GetFeedbacksByCaregiverId(int caregiverId)
        {
            return _feedbackRepository.GetFeedbacksByCaregiverId(caregiverId);
        }

        public List<Feedback> GetFeedbacksByCustomerId(int accountId)
        {
            return _feedbackRepository.GetFeedbacksByCustomerId(accountId);
        }

        public void AddFeedback(Feedback feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            // Validation
            if (feedback.Rating < 1 || feedback.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            if (feedback.CaregiverProfessionalism < 1 || feedback.CaregiverProfessionalism > 5)
            {
                throw new ArgumentException("Caregiver professionalism rating must be between 1 and 5");
            }

            if (feedback.ServiceQuality < 1 || feedback.ServiceQuality > 5)
            {
                throw new ArgumentException("Service quality rating must be between 1 and 5");
            }

            if (feedback.OverallExperience < 1 || feedback.OverallExperience > 5)
            {
                throw new ArgumentException("Overall experience rating must be between 1 and 5");
            }

            _feedbackRepository.AddFeedback(feedback);
        }

        public void UpdateFeedback(Feedback feedback)
        {
            if (feedback == null)
            {
                throw new ArgumentNullException(nameof(feedback));
            }

            // Validation
            if (feedback.Rating < 1 || feedback.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            if (feedback.CaregiverProfessionalism < 1 || feedback.CaregiverProfessionalism > 5)
            {
                throw new ArgumentException("Caregiver professionalism rating must be between 1 and 5");
            }

            if (feedback.ServiceQuality < 1 || feedback.ServiceQuality > 5)
            {
                throw new ArgumentException("Service quality rating must be between 1 and 5");
            }

            if (feedback.OverallExperience < 1 || feedback.OverallExperience > 5)
            {
                throw new ArgumentException("Overall experience rating must be between 1 and 5");
            }

            _feedbackRepository.UpdateFeedback(feedback);
        }

        public void DeleteFeedback(int id)
        {
            _feedbackRepository.DeleteFeedback(id);
        }

        public double GetAverageRatingForCaregiver(int caregiverId)
        {
            return _feedbackRepository.GetAverageRatingForCaregiver(caregiverId);
        }

        public Dictionary<string, double> GetDetailedRatingsForCaregiver(int caregiverId)
        {
            return _feedbackRepository.GetDetailedRatingsForCaregiver(caregiverId);
        }

        public bool CanCustomerAddFeedback(int customerId, int bookingId)
        {
            try
            {
                // Get the booking
                var booking = _bookingRepository.GetBookingById(bookingId);

                // Check if booking exists and belongs to this customer
                if (booking == null || booking.AccountId != customerId)
                {
                    return false;
                }

                // Check if booking is completed
                if (booking.Status != "completed")
                {
                    return false;
                }

                // Check if feedback already exists
                var existingFeedback = _feedbackRepository.GetFeedbacksByBookingId(bookingId);
                if (existingFeedback != null && existingFeedback.Any())
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SubmitFeedback(int bookingId, string note, int rating,
            int caregiverProfessionalism, int serviceQuality, int overallExperience)
        {
            // Validation
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            if (caregiverProfessionalism < 1 || caregiverProfessionalism > 5)
            {
                throw new ArgumentException("Caregiver professionalism rating must be between 1 and 5");
            }

            if (serviceQuality < 1 || serviceQuality > 5)
            {
                throw new ArgumentException("Service quality rating must be between 1 and 5");
            }

            if (overallExperience < 1 || overallExperience > 5)
            {
                throw new ArgumentException("Overall experience rating must be between 1 and 5");
            }

            // Use the stored procedure to add the feedback
            _feedbackRepository.AddFeedbackWithStoredProcedure(
                bookingId, note, rating, caregiverProfessionalism, serviceQuality, overallExperience);
        }
    }
}