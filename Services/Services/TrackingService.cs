using System;
using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class TrackingService : ITrackingService
    {
        private readonly ITrackingRepository _trackingRepository;

        public TrackingService(ITrackingRepository trackingRepository)
        {
            _trackingRepository = trackingRepository;
        }

        // Get all tracking records
        public List<Tracking> GetAllTrackings()
        {
            return _trackingRepository.GetAllTrackings();
        }

        // Get tracking records by Elder ID
        public List<Tracking> GetTrackingsByElderId(int elderId)
        {
            return _trackingRepository.GetTrackingsByElderId(elderId);
        }

        // Get a specific tracking record by ID
        public Tracking GetTrackingById(int trackingId)
        {
            return _trackingRepository.GetTrackingById(trackingId);
        }

        // Add a new tracking record
        public void AddTracking(Tracking tracking)
        {
            _trackingRepository.AddTracking(tracking);
        }

        // Update an existing tracking record
        public void UpdateTracking(Tracking tracking)
        {
            _trackingRepository.UpdateTracking(tracking);
        }

        // Delete a tracking record
        public void DeleteTracking(int trackingId)
        {
            _trackingRepository.DeleteTracking(trackingId);
        }

        // Get tracking records within a date range for a specific elder
        public List<Tracking> GetTrackingsByDateRange(int elderId, DateOnly startDate, DateOnly endDate)
        {
            return _trackingRepository.GetTrackingsByDateRange(elderId, startDate, endDate);
        }

        // Get latest tracking record for an elder
        public Tracking GetLatestTracking(int elderId)
        {
            return _trackingRepository.GetLatestTracking(elderId);
        }
    }
}