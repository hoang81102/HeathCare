using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class TrackingRepository : ITrackingRepository
    {
        private readonly TrackingDAO _trackingDAO;

        public TrackingRepository()
        {
            _trackingDAO = TrackingDAO.Instance;
        }

        // Get all tracking records
        public List<Tracking> GetAllTrackings()
        {
            return _trackingDAO.GetAllTrackings();
        }

        // Get tracking records by Elder ID
        public List<Tracking> GetTrackingsByElderId(int elderId)
        {
            return _trackingDAO.GetTrackingsByElderId(elderId);
        }

        // Get a specific tracking record by ID
        public Tracking GetTrackingById(int trackingId)
        {
            return _trackingDAO.GetTrackingById(trackingId);
        }

        // Add a new tracking record
        public void AddTracking(Tracking tracking)
        {
            _trackingDAO.AddTracking(tracking);
        }

        // Update an existing tracking record
        public void UpdateTracking(Tracking tracking)
        {
            _trackingDAO.UpdateTracking(tracking);
        }

        // Delete a tracking record
        public void DeleteTracking(int trackingId)
        {
            _trackingDAO.DeleteTracking(trackingId);
        }

        // Get tracking records within a date range for a specific elder
        public List<Tracking> GetTrackingsByDateRange(int elderId, DateOnly startDate, DateOnly endDate)
        {
            return _trackingDAO.GetTrackingsByDateRange(elderId, startDate, endDate);
        }

        // Get latest tracking record for an elder
        public Tracking GetLatestTracking(int elderId)
        {
            return _trackingDAO.GetLatestTracking(elderId);
        }
    }
}