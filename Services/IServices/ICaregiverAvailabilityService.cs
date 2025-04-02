using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface ICaregiverAvailabilityService
    {
        List<CaregiverAvailability> GetAllAvailabilities();
        List<CaregiverAvailability> GetAvailabilitiesByCaregiverId(int caregiverId);
        CaregiverAvailability GetAvailabilityById(int availabilityId);
        void AddAvailability(CaregiverAvailability availability);
        void UpdateAvailability(CaregiverAvailability availability);
        void DeleteAvailability(int availabilityId);
        List<Caregiver> GetAvailableCaregivers(DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime);
        List<(TimeOnly StartTime, TimeOnly EndTime)> GetAvailableTimeSlots(int caregiverId, DateOnly bookingDate);
        void ToggleAvailabilityStatus(int availabilityId, bool isAvailable);

        // Additional service methods
        Dictionary<int, string> GetDayOfWeekOptions();
        bool CheckTimeSlotAvailability(int caregiverId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime);
    }
}