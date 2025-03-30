using System;
using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class RecordService : IRecordService
    {
        private readonly IRecordRepository _recordRepository;

        public RecordService(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        // Get all Records
        public List<Record> GetAllRecords()
        {
            return _recordRepository.GetAllRecords();
        }

        // Get Record by ID
        public Record GetRecordById(int recordId)
        {
            return _recordRepository.GetRecordById(recordId);
        }

        // Get Records by Elder ID
        public List<Record> GetRecordsByElderId(int elderId)
        {
            return _recordRepository.GetRecordsByElderId(elderId);
        }

        // Get Records by Booking ID
        public Record GetRecordByBookingId(int bookingId)
        {
            return _recordRepository.GetRecordByBookingId(bookingId);
        }

        // Get Records by Caregiver ID (via Booking)
        public List<Record> GetRecordsByCaregiverId(int caregiverId)
        {
            return _recordRepository.GetRecordsByCaregiverId(caregiverId);
        }

        // Add new Record
        public void AddRecord(Record record)
        {
            _recordRepository.AddRecord(record);
        }

        // Update Record
        public void UpdateRecord(Record record)
        {
            _recordRepository.UpdateRecord(record);
        }

        // Delete Record
        public void DeleteRecord(int recordId)
        {
            _recordRepository.DeleteRecord(recordId);
        }

        // Clock In - Update record status to InProgress and set clock in time
        public void ClockIn(int recordId)
        {
            _recordRepository.ClockIn(recordId);
        }

        // Clock Out - Update record status to Finished and set clock out time
        public void ClockOut(int recordId)
        {
            _recordRepository.ClockOut(recordId);
        }

        // Update Guidelines
        public void UpdateGuidelines(int recordId, string exerciseGuidelines, string dietGuidelines, string otherGuidelines)
        {
            _recordRepository.UpdateGuidelines(recordId, exerciseGuidelines, dietGuidelines, otherGuidelines);
        }

        // Get Records by Status
        public List<Record> GetRecordsByStatus(string status)
        {
            return _recordRepository.GetRecordsByStatus(status);
        }

        // Get Records by Date Range
        public List<Record> GetRecordsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _recordRepository.GetRecordsByDateRange(startDate, endDate);
        }

        // Get Active Records (In Progress)
        public List<Record> GetActiveRecords()
        {
            return _recordRepository.GetActiveRecords();
        }

        // Get Completed Records
        public List<Record> GetCompletedRecords()
        {
            return _recordRepository.GetCompletedRecords();
        }

        // Get Records with Guidelines
        public List<Record> GetRecordsWithGuidelines()
        {
            return _recordRepository.GetRecordsWithGuidelines();
        }
    }
}