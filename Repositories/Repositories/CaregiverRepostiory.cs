using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class CaregiverRepository : ICaregiverRepository
    {
        private readonly CaregiverDAO _caregiverDAO;

        public CaregiverRepository()
        {
            _caregiverDAO = CaregiverDAO.Instance;
        }

        public List<Caregiver> GetAllCaregivers()
        {
            return _caregiverDAO.GetAllCaregivers();
        }

        public Caregiver GetCaregiverById(int caregiverId)
        {
            return _caregiverDAO.GetCaregiverById(caregiverId);
        }

        public Caregiver GetCaregiverByAccountId(int accountId)
        {
            return _caregiverDAO.GetCaregiverByAccountId(accountId);
        }

        public void AddCaregiver(Caregiver caregiver)
        {
            _caregiverDAO.AddCaregiver(caregiver);
        }

        public void UpdateCaregiver(Caregiver caregiver)
        {
            _caregiverDAO.UpdateCaregiver(caregiver);
        }

        public void DeleteCaregiver(int caregiverId)
        {
            _caregiverDAO.DeleteCaregiver(caregiverId);
        }

        public List<Caregiver> GetAvailableCaregivers(DateTime bookingDate, TimeSpan startTime, TimeSpan endTime)
        {
            return _caregiverDAO.GetAvailableCaregivers(bookingDate, startTime, endTime);
        }

        public void AddCaregiverAvailability(CaregiverAvailability availability)
        {
            _caregiverDAO.AddCaregiverAvailability(availability);
        }

        public void UpdateCaregiverAvailability(CaregiverAvailability availability)
        {
            _caregiverDAO.UpdateCaregiverAvailability(availability);
        }

        public void DeleteCaregiverAvailability(int availabilityId)
        {
            _caregiverDAO.DeleteCaregiverAvailability(availabilityId);
        }

        public List<CaregiverAvailability> GetCaregiverAvailability(int caregiverId, DateTime bookingDate)
        {
            return _caregiverDAO.GetCaregiverAvailability(caregiverId, bookingDate);
        }

        public List<CaregiverAvailability> GetAllAvailabilityForCaregiver(int caregiverId)
        {
            return _caregiverDAO.GetAllAvailabilityForCaregiver(caregiverId);
        }

        public List<Caregiver> GetCaregiversBySpecialty(string specialty)
        {
            return _caregiverDAO.GetCaregiversBySpecialty(specialty);
        }

        public List<Caregiver> GetTopRatedCaregivers(int limit = 5)
        {
            return _caregiverDAO.GetTopRatedCaregivers(limit);
        }
    }
}