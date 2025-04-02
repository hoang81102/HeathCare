using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface IMedicalRecordRepository
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