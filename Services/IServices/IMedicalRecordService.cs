using BusinessObjects;
using System.Collections.Generic;

namespace Services
{
    public interface IMedicalRecordService
    {
        List<MedicalRecord> GetAllMedicalRecords();
        List<MedicalRecord> GetMedicalRecordsByElderId(int elderId);
        MedicalRecord GetMedicalRecordById(int medicalRecordId);
        void AddMedicalRecord(MedicalRecord medicalRecord);
        void UpdateMedicalRecord(MedicalRecord medicalRecord);
        void DeleteMedicalRecord(int medicalRecordId);
        MedicalRecord GetLatestMedicalRecord(int elderId);
    }
}