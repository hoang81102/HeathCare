using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class CaregiverAvailabilityRepository : ICaregiverAvailabilityRepository
    {
        private readonly CaregiverAvailabilityDAO _caregiverAvailabilityDAO;

        public CaregiverAvailabilityRepository()
        {
            _caregiverAvailabilityDAO = CaregiverAvailabilityDAO.Instance;
        }

        public List<CaregiverAvailability> GetAllAvailabilities()
        {
            return _caregiverAvailabilityDAO.GetAllAvailabilities();
        }

        public List<CaregiverAvailability> GetAvailabilitiesByCaregiverId(int caregiverId)
        {
            return _caregiverAvailabilityDAO.GetAvailabilitiesByCaregiverId(caregiverId);
        }

        public CaregiverAvailability GetAvailabilityById(int availabilityId)
        {
            return _caregiverAvailabilityDAO.GetAvailabilityById(availabilityId);
        }

        public void AddAvailability(CaregiverAvailability availability)
        {
            _caregiverAvailabilityDAO.AddAvailability(availability);
        }

        public void UpdateAvailability(CaregiverAvailability availability)
        {
            _caregiverAvailabilityDAO.UpdateAvailability(availability);
        }

        public void DeleteAvailability(int availabilityId)
        {
            _caregiverAvailabilityDAO.DeleteAvailability(availabilityId);
        }

        public List<Caregiver> GetAvailableCaregivers(DateOnly bookingDate, TimeOnly startTime, TimeOnly endTime)
        {
            return _caregiverAvailabilityDAO.GetAvailableCaregivers(bookingDate, startTime, endTime);
        }

        public List<(TimeOnly StartTime, TimeOnly EndTime)> GetAvailableTimeSlots(int caregiverId, DateOnly bookingDate)
        {
            return _caregiverAvailabilityDAO.GetAvailableTimeSlots(caregiverId, bookingDate);
        }

        public void ToggleAvailabilityStatus(int availabilityId, bool isAvailable)
        {
            _caregiverAvailabilityDAO.ToggleAvailabilityStatus(availabilityId, isAvailable);
        }
    }
}