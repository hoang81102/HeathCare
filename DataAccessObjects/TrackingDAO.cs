using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class TrackingDAO : SingletonBase<TrackingDAO>
    {
        // Get all tracking records
        public List<Tracking> GetAllTrackings()
        {
            try
            {
                return _context.Trackings.Include(t => t.Elder).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving tracking records: " + ex.Message);
            }
        }

        // Get tracking records by Elder ID
        public List<Tracking> GetTrackingsByElderId(int elderId)
        {
            try
            {
                return _context.Trackings
                    .Where(t => t.ElderId == elderId)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving tracking records for elder ID {elderId}: " + ex.Message);
            }
        }

        // Get a specific tracking record by ID
        public Tracking GetTrackingById(int trackingId)
        {
            try
            {
                return _context.Trackings
                    .Include(t => t.Elder)
                    .FirstOrDefault(t => t.TrackingId == trackingId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving tracking record with ID {trackingId}: " + ex.Message);
            }
        }

        // Add a new tracking record
        public void AddTracking(Tracking tracking)
        {
            try
            {
                _context.Trackings.Add(tracking);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding tracking record: " + ex.Message);
            }
        }

        // Update an existing tracking record
        public void UpdateTracking(Tracking tracking)
        {
            try
            {
                var existingTracking = _context.Trackings.Find(tracking.TrackingId);
                if (existingTracking == null)
                {
                    throw new Exception($"Tracking record with ID {tracking.TrackingId} not found");
                }

                _context.Entry(existingTracking).CurrentValues.SetValues(tracking);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating tracking record: " + ex.Message);
            }
        }

        // Delete a tracking record
        public void DeleteTracking(int trackingId)
        {
            try
            {
                var tracking = _context.Trackings.Find(trackingId);
                if (tracking != null)
                {
                    _context.Trackings.Remove(tracking);
                    _context.SaveChanges();
                }
                else
                {
                    throw new Exception($"Tracking record with ID {trackingId} not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting tracking record: " + ex.Message);
            }
        }

        // Get tracking records within a date range for a specific elder
        public List<Tracking> GetTrackingsByDateRange(int elderId, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                return _context.Trackings
                    .Where(t => t.ElderId == elderId && t.Date >= startDate && t.Date <= endDate)
                    .OrderByDescending(t => t.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving tracking records for elder ID {elderId} by date range: " + ex.Message);
            }
        }

        // Get latest tracking record for an elder
        public Tracking GetLatestTracking(int elderId)
        {
            try
            {
                return _context.Trackings
                    .Where(t => t.ElderId == elderId)
                    .OrderByDescending(t => t.Date)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving latest tracking record for elder ID {elderId}: " + ex.Message);
            }
        }
    }
}