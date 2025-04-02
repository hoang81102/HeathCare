using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class CaregiverAvailabilityService : ICaregiverAvailabilityService
    {
        private readonly ICaregiverAvailabilityRepository _caregiverAvailabilityRepository;

        public CaregiverAvailabilityService(ICaregiverAvailabilityRepository caregiverAvailabilityRepository)
        {
            _caregiverAvailabilityRepository = caregiverAvailabilityRepository;
        }

        public List<CaregiverAvailability> GetAllAvailabilities()
        {
            return _caregiverAvailabilityRepository.GetAllAvailabilities();
        }

        public List<CaregiverAvailability> GetAvailabilitiesByCaregiverId(int caregiverId)
        {
            return _caregiverAvailabilityRepository.GetAvailabilitiesByCaregiverId(caregiverId);
        }

        public CaregiverAvailability GetAvailabilityById(int availabilityId)
        {
            return _caregiverAvailabilityRepository.GetAvailabilityById(availabilityId);
        }

        public void AddAvailability(CaregiverAvailability availability)
        {
            // Add any additional business rules or validation here
            if (availability.IsAvailable == null)
            {
                availability.IsAvailable = true; // Set default value if not provided
            }

            _caregiverAvailabilityRepository.AddAvailability(availability);
        }

        public void UpdateAvailability(CaregiverAvailability availability)
        {
            // Add any additional business rules or validation here
            _caregiverAvailabilityRepository.UpdateAvailability(availability);
        }

        public void DeleteAvailability(int availabilityId)
        {
            // Add any additional business rules or validation here
            _caregiverAvailabilityRepository.DeleteAvailability(availabilityId);
        }

        public List<Caregiver> GetAvailableCaregivers(DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            // Validate input parameters
            if (startTime >= endTime)
            {
                throw new ArgumentException("Start time must be before end time");
            }

            return _caregiverAvailabilityRepository.GetAvailableCaregivers(bookingDate, startTime, endTime);
        }

        public List<(TimeOnly StartTime, TimeOnly EndTime)> GetAvailableTimeSlots(int caregiverId, DateOnly bookingDate)
        {
            return _caregiverAvailabilityRepository.GetAvailableTimeSlots(caregiverId, bookingDate);
        }

        public void ToggleAvailabilityStatus(int availabilityId, bool isAvailable)
        {
            _caregiverAvailabilityRepository.ToggleAvailabilityStatus(availabilityId, isAvailable);
        }

        // Additional service methods

        public Dictionary<int, string> GetDayOfWeekOptions()
        {
            return new Dictionary<int, string>
            {
                { 1, "Monday" },
                { 2, "Tuesday" },
                { 3, "Wednesday" },
                { 4, "Thursday" },
                { 5, "Friday" },
                { 6, "Saturday" },
                { 7, "Sunday" }
            };
        }

        public bool CheckTimeSlotAvailability(int caregiverId, DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            // Check if the provided time slot is available for booking
            var availableSlots = GetAvailableTimeSlots(caregiverId, bookingDate);

            // Check if there is any available slot that contains the requested time range
            return availableSlots.Any(slot =>
                slot.StartTime <= startTime && slot.EndTime >= endTime);
        }
    }
}