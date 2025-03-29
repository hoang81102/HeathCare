using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class RecordDAO : SingletonBase<RecordDAO>
    {
        // Get all Records
        public List<Record> GetAllRecords()
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving records: " + ex.Message);
            }
        }

        // Get Record by ID
        public Record GetRecordById(int recordId)
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .FirstOrDefault(r => r.RecordId == recordId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving record: " + ex.Message);
            }
        }

        // Get Records by Elder ID
        public List<Record> GetRecordsByElderId(int elderId)
        {
            try
            {
                return _context.Records
                    .Include(r => r.Booking)
                    .Where(r => r.ElderId == elderId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving records for elder: " + ex.Message);
            }
        }

        // Get Records by Booking ID
        public Record GetRecordByBookingId(int bookingId)
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .FirstOrDefault(r => r.BookingId == bookingId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving record for booking: " + ex.Message);
            }
        }

        // Get Records by Caregiver ID (via Booking)
        public List<Record> GetRecordsByCaregiverId(int caregiverId)
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .Where(r => r.Booking.CaregiverId == caregiverId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving records for caregiver: " + ex.Message);
            }
        }

        // Add new Record
        public void AddRecord(Record record)
        {
            try
            {
                _context.Records.Add(record);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding record: " + ex.Message);
            }
        }

        // Update Record
        public void UpdateRecord(Record record)
        {
            try
            {
                var existingRecord = _context.Records.Find(record.RecordId);
                if (existingRecord == null)
                {
                    throw new Exception("Record not found");
                }

                // Update all fields
                existingRecord.Description = record.Description;
                existingRecord.Status = record.Status;
                existingRecord.LastUpdated = DateTime.Now;
                existingRecord.ClockInTime = record.ClockInTime;
                existingRecord.ClockOutTime = record.ClockOutTime;
                existingRecord.ExerciseGuidelines = record.ExerciseGuidelines;
                existingRecord.DietGuidelines = record.DietGuidelines;
                existingRecord.OtherGuidelines = record.OtherGuidelines;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating record: " + ex.Message);
            }
        }

        // Delete Record
        public void DeleteRecord(int recordId)
        {
            try
            {
                var record = _context.Records.Find(recordId);
                if (record != null)
                {
                    _context.Records.Remove(record);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting record: " + ex.Message);
            }
        }

        // Clock In - Update record status to InProgress and set clock in time
        public void ClockIn(int recordId)
        {
            try
            {
                var record = _context.Records.Find(recordId);
                if (record == null)
                {
                    throw new Exception("Record not found");
                }

                record.Status = "InProgress";
                record.ClockInTime = DateTime.Now;
                record.LastUpdated = DateTime.Now;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error clocking in: " + ex.Message);
            }
        }

        // Clock Out - Update record status to Finished and set clock out time
        public void ClockOut(int recordId)
        {
            try
            {
                var record = _context.Records.Find(recordId);
                if (record == null)
                {
                    throw new Exception("Record not found");
                }

                record.Status = "Finished";
                record.ClockOutTime = DateTime.Now;
                record.LastUpdated = DateTime.Now;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error clocking out: " + ex.Message);
            }
        }

        // Update Guidelines
        public void UpdateGuidelines(int recordId, string exerciseGuidelines, string dietGuidelines, string otherGuidelines)
        {
            try
            {
                var record = _context.Records.Find(recordId);
                if (record == null)
                {
                    throw new Exception("Record not found");
                }

                record.ExerciseGuidelines = exerciseGuidelines;
                record.DietGuidelines = dietGuidelines;
                record.OtherGuidelines = otherGuidelines;
                record.LastUpdated = DateTime.Now;

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating guidelines: " + ex.Message);
            }
        }

        // Get Records by Status
        public List<Record> GetRecordsByStatus(string status)
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .Where(r => r.Status == status)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving records by status: " + ex.Message);
            }
        }

        // Get Records by Date Range
        public List<Record> GetRecordsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .Where(r => r.LastUpdated >= startDate && r.LastUpdated <= endDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving records by date range: " + ex.Message);
            }
        }

        // Get Active Records (In Progress)
        public List<Record> GetActiveRecords()
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .Where(r => r.Status == "InProgress")
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving active records: " + ex.Message);
            }
        }

        // Get Completed Records
        public List<Record> GetCompletedRecords()
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .Where(r => r.Status == "Finished")
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving completed records: " + ex.Message);
            }
        }

        // Get Records with Guidelines
        public List<Record> GetRecordsWithGuidelines()
        {
            try
            {
                return _context.Records
                    .Include(r => r.Elder)
                    .Include(r => r.Booking)
                    .Where(r => r.ExerciseGuidelines != null || r.DietGuidelines != null || r.OtherGuidelines != null)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving records with guidelines: " + ex.Message);
            }
        }
    }
}