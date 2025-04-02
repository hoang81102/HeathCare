using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class CaregiverService : ICaregiverService
    {
        private readonly ICaregiverRepository _caregiverRepository;

        public CaregiverService(ICaregiverRepository caregiverRepository)
        {
            _caregiverRepository = caregiverRepository;
        }

        public List<Caregiver> GetAllCaregivers()
        {
            return _caregiverRepository.GetAllCaregivers();
        }

        public Caregiver GetCaregiverById(int caregiverId)
        {
            return _caregiverRepository.GetCaregiverById(caregiverId);
        }

        public Caregiver GetCaregiverByAccountId(int accountId)
        {
            return _caregiverRepository.GetCaregiverByAccountId(accountId);
        }

        public void AddCaregiver(Caregiver caregiver)
        {
            // Validate caregiver data before adding
            if (string.IsNullOrEmpty(caregiver.Availability))
            {
                throw new ArgumentException("Caregiver availability cannot be empty");
            }

            if (caregiver.ExperienceYears < 0)
            {
                throw new ArgumentException("Experience years cannot be negative");
            }

            _caregiverRepository.AddCaregiver(caregiver);
        }

        public void UpdateCaregiver(Caregiver caregiver)
        {
            // Validate caregiver data before updating
            if (string.IsNullOrEmpty(caregiver.Availability))
            {
                throw new ArgumentException("Caregiver availability cannot be empty");
            }

            if (caregiver.ExperienceYears < 0)
            {
                throw new ArgumentException("Experience years cannot be negative");
            }

            _caregiverRepository.UpdateCaregiver(caregiver);
        }

        public void DeleteCaregiver(int caregiverId)
        {
            // Check if caregiver exists before deleting
            var caregiver = _caregiverRepository.GetCaregiverById(caregiverId);
            if (caregiver == null)
            {
                throw new ArgumentException($"Caregiver with ID {caregiverId} not found");
            }

            _caregiverRepository.DeleteCaregiver(caregiverId);
        }

        public List<Caregiver> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            // Validate input parameters
            if (bookingDate.Date < DateTime.Today)
            {
                throw new ArgumentException("Booking date cannot be in the past");
            }

            if (startTime >= endTime)
            {
                throw new ArgumentException("Start time must be before end time");
            }

            return _caregiverRepository.GetAvailableCaregivers(bookingDate, startTime, endTime);
        }

        public void AddCaregiverAvailability(CaregiverAvailability availability)
        {
            // Validate availability data
            if (availability.StartTime >= availability.EndTime)
            {
                throw new ArgumentException("Start time must be before end time");
            }

            // Check if caregiver exists
            var caregiver = _caregiverRepository.GetCaregiverById(availability.CaregiverId);
            if (caregiver == null)
            {
                throw new ArgumentException($"Caregiver with ID {availability.CaregiverId} not found");
            }

            _caregiverRepository.AddCaregiverAvailability(availability);
        }

        public void UpdateCaregiverAvailability(CaregiverAvailability availability)
        {
            // Validate availability data
            if (availability.StartTime >= availability.EndTime)
            {
                throw new ArgumentException("Start time must be before end time");
            }

            _caregiverRepository.UpdateCaregiverAvailability(availability);
        }

        public void DeleteCaregiverAvailability(int availabilityId)
        {
            _caregiverRepository.DeleteCaregiverAvailability(availabilityId);
        }

        public List<CaregiverAvailability> GetCaregiverAvailability(int caregiverId, DateTime bookingDate)
        {
            return _caregiverRepository.GetCaregiverAvailability(caregiverId, bookingDate);
        }

        public List<CaregiverAvailability> GetAllAvailabilityForCaregiver(int caregiverId)
        {
            return _caregiverRepository.GetAllAvailabilityForCaregiver(caregiverId);
        }

        public List<Caregiver> GetCaregiversBySpecialty(string specialty)
        {
            if (string.IsNullOrEmpty(specialty))
            {
                throw new ArgumentException("Specialty cannot be empty");
            }

            return _caregiverRepository.GetCaregiversBySpecialty(specialty);
        }

        public List<Caregiver> GetTopRatedCaregivers(int limit = 5)
        {
            if (limit <= 0)
            {
                throw new ArgumentException("Limit must be greater than zero");
            }

            return _caregiverRepository.GetTopRatedCaregivers(limit);
        }

        
    }
}