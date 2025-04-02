using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly RecordDAO _recordDAO;

        public RecordRepository()
        {
            _recordDAO = RecordDAO.Instance;
        }

        // Get all Records
        public List<Record> GetAllRecords()
        {
            return _recordDAO.GetAllRecords();
        }

        // Get Record by ID
        public Record GetRecordById(int recordId)
        {
            return _recordDAO.GetRecordById(recordId);
        }

        // Get Records by Elder ID
        public List<Record> GetRecordsByElderId(int elderId)
        {
            return _recordDAO.GetRecordsByElderId(elderId);
        }

        // Get Records by Booking ID
        public Record GetRecordByBookingId(int bookingId)
        {
            return _recordDAO.GetRecordByBookingId(bookingId);
        }

        // Get Records by Caregiver ID (via Booking)
        public List<Record> GetRecordsByCaregiverId(int caregiverId)
        {
            return _recordDAO.GetRecordsByCaregiverId(caregiverId);
        }

        // Add new Record
        public void AddRecord(Record record)
        {
            _recordDAO.AddRecord(record);
        }

        // Update Record
        public void UpdateRecord(Record record)
        {
            _recordDAO.UpdateRecord(record);
        }

        // Delete Record
        public void DeleteRecord(int recordId)
        {
            _recordDAO.DeleteRecord(recordId);
        }

        // Clock In - Update record status to InProgress and set clock in time
        public void ClockIn(int recordId)
        {
            _recordDAO.ClockIn(recordId);
        }

        // Clock Out - Update record status to Finished and set clock out time
        public void ClockOut(int recordId)
        {
            _recordDAO.ClockOut(recordId);
        }

        // Update Guidelines
        public void UpdateGuidelines(int recordId, string exerciseGuidelines, string dietGuidelines, string otherGuidelines)
        {
            _recordDAO.UpdateGuidelines(recordId, exerciseGuidelines, dietGuidelines, otherGuidelines);
        }

        // Get Records by Status
        public List<Record> GetRecordsByStatus(string status)
        {
            return _recordDAO.GetRecordsByStatus(status);
        }

        // Get Records by Date Range
        public List<Record> GetRecordsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _recordDAO.GetRecordsByDateRange(startDate, endDate);
        }

        // Get Active Records (In Progress)
        public List<Record> GetActiveRecords()
        {
            return _recordDAO.GetActiveRecords();
        }

        // Get Completed Records
        public List<Record> GetCompletedRecords()
        {
            return _recordDAO.GetCompletedRecords();
        }

        // Get Records with Guidelines
        public List<Record> GetRecordsWithGuidelines()
        {
            return _recordDAO.GetRecordsWithGuidelines();
        }
    }
}