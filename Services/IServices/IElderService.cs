using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IElderService
    {
        // Get all elders
        List<Elder> GetAllElders();

        // Get elder by ID
        Elder GetElderById(int elderId);

        // Get elders by account ID (customer)
        List<Elder> GetEldersByAccountId(int accountId);

        // Add a new elder
        void AddElder(Elder elder);

        // Update an elder
        void UpdateElder(Elder elder);

        // Delete an elder
        void DeleteElder(int elderId);

        // Get elder with medical records
        Elder GetElderWithMedicalRecords(int elderId);

        // Get elder with tracking information
        Elder GetElderWithTrackingInfo(int elderId);

        // Get elder with booking history
        Elder GetElderWithBookingHistory(int elderId);

        // Get elder with records
        Elder GetElderWithRecords(int elderId);

        // Search elders by name
        List<Elder> SearchEldersByName(string name);
    }
}