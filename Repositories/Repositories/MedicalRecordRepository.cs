using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class MedicalRecordRepository : IMedicalRecordRepository
    {
        private readonly MedicalRecordDAO _medicalRecordDAO;

        public MedicalRecordRepository()
        {
            _medicalRecordDAO = MedicalRecordDAO.Instance;
        }

        public List<MedicalRecord> GetAllMedicalRecords()
        {
            return _medicalRecordDAO.GetAllMedicalRecords();
        }

        public List<MedicalRecord> GetMedicalRecordsByElderId(int elderId)
        {
            return _medicalRecordDAO.GetMedicalRecordsByElderId(elderId);
        }

        public MedicalRecord GetMedicalRecordById(int medicalRecordId)
        {
            return _medicalRecordDAO.GetMedicalRecordById(medicalRecordId);
        }

        public void AddMedicalRecord(MedicalRecord medicalRecord)
        {
            _medicalRecordDAO.AddMedicalRecord(medicalRecord);
        }

        public void UpdateMedicalRecord(MedicalRecord medicalRecord)
        {
            _medicalRecordDAO.UpdateMedicalRecord(medicalRecord);
        }

        public void DeleteMedicalRecord(int medicalRecordId)
        {
            _medicalRecordDAO.DeleteMedicalRecord(medicalRecordId);
        }

        public MedicalRecord GetLatestMedicalRecord(int elderId)
        {
            return _medicalRecordDAO.GetLatestMedicalRecord(elderId);
        }
    }
}