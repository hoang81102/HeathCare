using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IRecordService
    {
        // Get all Records
        List<Record> GetAllRecords();

        // Get Record by ID
        Record GetRecordById(int recordId);

        // Get Records by Elder ID
        List<Record> GetRecordsByElderId(int elderId);

        // Get Records by Booking ID
        Record GetRecordByBookingId(int bookingId);

        // Get Records by Caregiver ID (via Booking)
        List<Record> GetRecordsByCaregiverId(int caregiverId);

        // Add new Record
        void AddRecord(Record record);

        // Update Record
        void UpdateRecord(Record record);

        // Delete Record
        void DeleteRecord(int recordId);

        // Clock In - Update record status to InProgress and set clock in time
        void ClockIn(int recordId);

        // Clock Out - Update record status to Finished and set clock out time
        void ClockOut(int recordId);

        // Update Guidelines
        void UpdateGuidelines(int recordId, string exerciseGuidelines, string dietGuidelines, string otherGuidelines);

        // Get Records by Status
        List<Record> GetRecordsByStatus(string status);

        // Get Records by Date Range
        List<Record> GetRecordsByDateRange(DateTime startDate, DateTime endDate);

        // Get Active Records (In Progress)
        List<Record> GetActiveRecords();

        // Get Completed Records
        List<Record> GetCompletedRecords();

        // Get Records with Guidelines
        List<Record> GetRecordsWithGuidelines();
    }
}