using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IFeedbackService
    {
        List<Feedback> GetAllFeedbacks();
        Feedback GetFeedbackById(int id);
        List<Feedback> GetFeedbacksByBookingId(int bookingId);
        List<Feedback> GetFeedbacksByCaregiverId(int caregiverId);
        List<Feedback> GetFeedbacksByCustomerId(int accountId);
        void AddFeedback(Feedback feedback);
        void UpdateFeedback(Feedback feedback);
        void DeleteFeedback(int id);
        double GetAverageRatingForCaregiver(int caregiverId);
        Dictionary<string, double> GetDetailedRatingsForCaregiver(int caregiverId);
        bool CanCustomerAddFeedback(int customerId, int bookingId);
        void SubmitFeedback(int bookingId, string note, int rating,
            int caregiverProfessionalism, int serviceQuality, int overallExperience);
    }
}