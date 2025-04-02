using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface ICaregiverService
    {
        // Basic CRUD operations
        List<Caregiver> GetAllCaregivers();
        Caregiver GetCaregiverById(int caregiverId);
        Caregiver GetCaregiverByAccountId(int accountId);
        void AddCaregiver(Caregiver caregiver);
        void UpdateCaregiver(Caregiver caregiver);
        void DeleteCaregiver(int caregiverId);

        // Availability operations
        List<Caregiver> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime);
        void AddCaregiverAvailability(CaregiverAvailability availability);
        void UpdateCaregiverAvailability(CaregiverAvailability availability);
        void DeleteCaregiverAvailability(int availabilityId);
        List<CaregiverAvailability> GetCaregiverAvailability(int caregiverId, DateTime bookingDate);
        List<CaregiverAvailability> GetAllAvailabilityForCaregiver(int caregiverId);

        // Specialized queries
        List<Caregiver> GetCaregiversBySpecialty(string specialty);
        List<Caregiver> GetTopRatedCaregivers(int limit = 5);

    }
}