using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly FeedbackDAO _feedbackDAO;

        public FeedbackRepository()
        {
            _feedbackDAO = FeedbackDAO.Instance;
        }

        public List<Feedback> GetAllFeedbacks()
        {
            return _feedbackDAO.GetAllFeedbacks();
        }

        public Feedback GetFeedbackById(int id)
        {
            return _feedbackDAO.GetFeedbackById(id);
        }

        public List<Feedback> GetFeedbacksByBookingId(int bookingId)
        {
            return _feedbackDAO.GetFeedbacksByBookingId(bookingId);
        }

        public List<Feedback> GetFeedbacksByCaregiverId(int caregiverId)
        {
            return _feedbackDAO.GetFeedbacksByCaregiverId(caregiverId);
        }

        public List<Feedback> GetFeedbacksByCustomerId(int accountId)
        {
            return _feedbackDAO.GetFeedbacksByCustomerId(accountId);
        }

        public void AddFeedback(Feedback feedback)
        {
            _feedbackDAO.AddFeedback(feedback);
        }

        public void UpdateFeedback(Feedback feedback)
        {
            _feedbackDAO.UpdateFeedback(feedback);
        }

        public void DeleteFeedback(int id)
        {
            _feedbackDAO.DeleteFeedback(id);
        }

        public double GetAverageRatingForCaregiver(int caregiverId)
        {
            return _feedbackDAO.GetAverageRatingForCaregiver(caregiverId);
        }

        public Dictionary<string, double> GetDetailedRatingsForCaregiver(int caregiverId)
        {
            return _feedbackDAO.GetDetailedRatingsForCaregiver(caregiverId);
        }

        public void AddFeedbackWithStoredProcedure(int bookingId, string note, int rating,
            int caregiverProfessionalism, int serviceQuality, int overallExperience)
        {
            _feedbackDAO.AddFeedbackWithStoredProcedure(
                bookingId, note, rating, caregiverProfessionalism, serviceQuality, overallExperience);
        }
    }
}