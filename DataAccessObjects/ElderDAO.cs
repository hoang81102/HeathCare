using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace DataAccessObjects
{
    public class ElderDAO : SingletonBase<ElderDAO>
    {
        // Get all elders
        public List<Elder> GetAllElders()
        {
            try
            {
                return _context.Elders.Include(e => e.Account).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting all elders: " + ex.Message);
            }
        }

        // Get elder by ID
        public Elder GetElderById(int elderId)
        {
            try
            {
                return _context.Elders
                    .Include(e => e.Account)
                    .FirstOrDefault(e => e.ElderId == elderId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting elder by ID: " + ex.Message);
            }
        }

        // Get elders by account ID (customer)
        public List<Elder> GetEldersByAccountId(int accountId)
        {
            try
            {
                return _context.Elders
                    .Where(e => e.AccountId == accountId)
                    .Include(e => e.Account)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting elders by account ID: " + ex.Message);
            }
        }

        // Add a new elder
        public void AddElder(Elder elder)
        {
            try
            {
                _context.Elders.Add(elder);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding elder: " + ex.Message);
            }
        }

        // Update an elder
        public void UpdateElder(Elder elder)
        {
            try
            {
                var existingElder = _context.Elders.Find(elder.ElderId);
                if (existingElder != null)
                {
                    _context.Entry(existingElder).CurrentValues.SetValues(elder);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Elder not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating elder: " + ex.Message);
            }
        }

        // Delete an elder
        public void DeleteElder(int elderId)
        {
            try
            {
                var elder = _context.Elders.Find(elderId);
                if (elder != null)
                {
                    _context.Elders.Remove(elder);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception("Elder not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting elder: " + ex.Message);
            }
        }

        // Get elder with medical records
        public Elder GetElderWithMedicalRecords(int elderId)
        {
            try
            {
                return _context.Elders
                    .Include(e => e.MedicalRecords)
                    .FirstOrDefault(e => e.ElderId == elderId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting elder with medical records: " + ex.Message);
            }
        }

        // Get elder with tracking information
        public Elder GetElderWithTrackingInfo(int elderId)
        {
            try
            {
                return _context.Elders
                    .Include(e => e.Trackings)
                    .FirstOrDefault(e => e.ElderId == elderId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting elder with tracking info: " + ex.Message);
            }
        }

        // Get elder with booking history
        public Elder GetElderWithBookingHistory(int elderId)
        {
            try
            {
                return _context.Elders
                    .Include(e => e.Bookings)
                        .ThenInclude(b => b.Service)
                    .Include(e => e.Bookings)
                        .ThenInclude(b => b.Caregiver)
                            .ThenInclude(c => c.Account)
                    .FirstOrDefault(e => e.ElderId == elderId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting elder with booking history: " + ex.Message);
            }
        }

        // Get elder with records
        public Elder GetElderWithRecords(int elderId)
        {
            try
            {
                return _context.Elders
                    .Include(e => e.Records)
                        .ThenInclude(r => r.Booking)
                    .FirstOrDefault(e => e.ElderId == elderId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting elder with records: " + ex.Message);
            }
        }

        // Search elders by name
        public List<Elder> SearchEldersByName(string name)
        {
            try
            {
                return _context.Elders
                    .Where(e => e.Fullname.Contains(name))
                    .Include(e => e.Account)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching elders by name: " + ex.Message);
            }
        }
    }
}