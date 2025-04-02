using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class MedicalRecordDAO : SingletonBase<MedicalRecordDAO>
    {
        // Get all medical records
        public List<MedicalRecord> GetAllMedicalRecords()
        {
            try
            {
                return _context.MedicalRecords.Include(m => m.Elder).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving medical records: " + ex.Message);
            }
        }

        // Get medical records by Elder ID
        public List<MedicalRecord> GetMedicalRecordsByElderId(int elderId)
        {
            try
            {
                return _context.MedicalRecords
                    .Where(m => m.ElderId == elderId)
                    .OrderByDescending(m => m.RecordDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving medical records for elder ID {elderId}: " + ex.Message);
            }
        }

        // Get a specific medical record by ID
        public MedicalRecord GetMedicalRecordById(int medicalRecordId)
        {
            try
            {
                return _context.MedicalRecords
                    .Include(m => m.Elder)
                    .FirstOrDefault(m => m.MedicalRecordId == medicalRecordId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving medical record with ID {medicalRecordId}: " + ex.Message);
            }
        }

        // Add a new medical record
        public void AddMedicalRecord(MedicalRecord medicalRecord)
        {
            try
            {
                _context.MedicalRecords.Add(medicalRecord);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding medical record: " + ex.Message);
            }
        }

        // Update an existing medical record
        public void UpdateMedicalRecord(MedicalRecord medicalRecord)
        {
            try
            {
                var existingRecord = _context.MedicalRecords.Find(medicalRecord.MedicalRecordId);
                if (existingRecord == null)
                {
                    throw new Exception($"Medical Record with ID {medicalRecord.MedicalRecordId} not found");
                }

                _context.Entry(existingRecord).CurrentValues.SetValues(medicalRecord);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating medical record: " + ex.Message);
            }
        }

        // Delete a medical record
        public void DeleteMedicalRecord(int medicalRecordId)
        {
            try
            {
                var medicalRecord = _context.MedicalRecords.Find(medicalRecordId);
                if (medicalRecord != null)
                {
                    _context.MedicalRecords.Remove(medicalRecord);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception($"Medical Record with ID {medicalRecordId} not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting medical record: " + ex.Message);
            }
        }

        // Get latest medical record for an elder
        public MedicalRecord GetLatestMedicalRecord(int elderId)
        {
            try
            {
                return _context.MedicalRecords
                    .Where(m => m.ElderId == elderId)
                    .OrderByDescending(m => m.RecordDate)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving latest medical record for elder ID {elderId}: " + ex.Message);
            }
        }
    }
}