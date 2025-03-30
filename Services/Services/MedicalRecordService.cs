using BusinessObjects;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordService(IMedicalRecordRepository medicalRecordRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
        }

        public List<MedicalRecord> GetAllMedicalRecords()
        {
            return _medicalRecordRepository.GetAllMedicalRecords();
        }

        public List<MedicalRecord> GetMedicalRecordsByElderId(int elderId)
        {
            return _medicalRecordRepository.GetMedicalRecordsByElderId(elderId);
        }

        public MedicalRecord GetMedicalRecordById(int medicalRecordId)
        {
            return _medicalRecordRepository.GetMedicalRecordById(medicalRecordId);
        }

        public void AddMedicalRecord(MedicalRecord medicalRecord)
        {
            _medicalRecordRepository.AddMedicalRecord(medicalRecord);
        }

        public void UpdateMedicalRecord(MedicalRecord medicalRecord)
        {
            _medicalRecordRepository.UpdateMedicalRecord(medicalRecord);
        }

        public void DeleteMedicalRecord(int medicalRecordId)
        {
            _medicalRecordRepository.DeleteMedicalRecord(medicalRecordId);
        }

        public MedicalRecord GetLatestMedicalRecord(int elderId)
        {
            return _medicalRecordRepository.GetLatestMedicalRecord(elderId);
        }
    }
}