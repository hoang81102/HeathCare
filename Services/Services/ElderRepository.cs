using System;
using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class ElderService : IElderService
    {
        private readonly IElderRepository _elderRepository;

        public ElderService(IElderRepository elderRepository)
        {
            _elderRepository = elderRepository;
        }

        // Get all elders
        public List<Elder> GetAllElders()
        {
            return _elderRepository.GetAllElders();
        }

        // Get elder by ID
        public Elder GetElderById(int elderId)
        {
            return _elderRepository.GetElderById(elderId);
        }

        // Get elders by account ID (customer)
        public List<Elder> GetEldersByAccountId(int accountId)
        {
            return _elderRepository.GetEldersByAccountId(accountId);
        }

        // Add a new elder
        public void AddElder(Elder elder)
        {
            _elderRepository.AddElder(elder);
        }

        // Update an elder
        public void UpdateElder(Elder elder)
        {
            _elderRepository.UpdateElder(elder);
        }

        // Delete an elder
        public void DeleteElder(int elderId)
        {
            _elderRepository.DeleteElder(elderId);
        }

        // Get elder with medical records
        public Elder GetElderWithMedicalRecords(int elderId)
        {
            return _elderRepository.GetElderWithMedicalRecords(elderId);
        }

        // Get elder with tracking information
        public Elder GetElderWithTrackingInfo(int elderId)
        {
            return _elderRepository.GetElderWithTrackingInfo(elderId);
        }

        // Get elder with booking history
        public Elder GetElderWithBookingHistory(int elderId)
        {
            return _elderRepository.GetElderWithBookingHistory(elderId);
        }

        // Get elder with records
        public Elder GetElderWithRecords(int elderId)
        {
            return _elderRepository.GetElderWithRecords(elderId);
        }

        // Search elders by name
        public List<Elder> SearchEldersByName(string name)
        {
            return _elderRepository.SearchEldersByName(name);
        }
    }
}