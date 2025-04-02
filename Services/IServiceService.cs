using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IServiceService
    {
        // Get all services
        List<Service> GetAllServices();

        // Get service by ID
        Service GetServiceById(int serviceId);

        // Get services by category
        List<Service> GetServicesByCategory(int categoryId);

        // Add a new service
        void AddService(Service service);

        // Update an existing service
        void UpdateService(Service service);

        // Delete a service
        void DeleteService(int serviceId);

    }
}