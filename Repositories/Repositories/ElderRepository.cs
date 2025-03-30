using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class ElderRepository : IElderRepository
    {
        private readonly ElderDAO _elderDAO;

        public ElderRepository()
        {
            _elderDAO = ElderDAO.Instance;
        }

        // Get all elders
        public List<Elder> GetAllElders()
        {
            return _elderDAO.GetAllElders();
        }

        // Get elder by ID
        public Elder GetElderById(int elderId)
        {
            return _elderDAO.GetElderById(elderId);
        }

        // Get elders by account ID (customer)
        public List<Elder> GetEldersByAccountId(int accountId)
        {
            return _elderDAO.GetEldersByAccountId(accountId);
        }

        // Add a new elder
        public void AddElder(Elder elder)
        {
            _elderDAO.AddElder(elder);
        }

        // Update an elder
        public void UpdateElder(Elder elder)
        {
            _elderDAO.UpdateElder(elder);
        }

        // Delete an elder
        public void DeleteElder(int elderId)
        {
            _elderDAO.DeleteElder(elderId);
        }

        // Get elder with medical records
        public Elder GetElderWithMedicalRecords(int elderId)
        {
            return _elderDAO.GetElderWithMedicalRecords(elderId);
        }

        // Get elder with tracking information
        public Elder GetElderWithTrackingInfo(int elderId)
        {
            return _elderDAO.GetElderWithTrackingInfo(elderId);
        }

        // Get elder with booking history
        public Elder GetElderWithBookingHistory(int elderId)
        {
            return _elderDAO.GetElderWithBookingHistory(elderId);
        }

        // Get elder with records
        public Elder GetElderWithRecords(int elderId)
        {
            return _elderDAO.GetElderWithRecords(elderId);
        }

        // Search elders by name
        public List<Elder> SearchEldersByName(string name)
        {
            return _elderDAO.SearchEldersByName(name);
        }
    }
}