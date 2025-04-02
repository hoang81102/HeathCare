using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface ITrackingRepository
    {
        // Get all tracking records
        List<Tracking> GetAllTrackings();

        // Get tracking records by Elder ID
        List<Tracking> GetTrackingsByElderId(int elderId);

        // Get a specific tracking record by ID
        Tracking GetTrackingById(int trackingId);

        // Add a new tracking record
        void AddTracking(Tracking tracking);

        // Update an existing tracking record
        void UpdateTracking(Tracking tracking);

        // Delete a tracking record
        void DeleteTracking(int trackingId);

        // Get tracking records within a date range for a specific elder
        List<Tracking> GetTrackingsByDateRange(int elderId, DateOnly startDate, DateOnly endDate);

        // Get latest tracking record for an elder
        Tracking GetLatestTracking(int elderId);
    }
}