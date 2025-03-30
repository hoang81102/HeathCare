using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ServiceDAO _serviceDAO;

        public ServiceRepository()
        {
            _serviceDAO = ServiceDAO.Instance;
        }

        // Get all services
        public List<Service> GetAllServices()
        {
            return _serviceDAO.GetAllServices();
        }

        // Get service by ID
        public Service GetServiceById(int serviceId)
        {
            return _serviceDAO.GetServiceById(serviceId);
        }

        // Get services by category
        public List<Service> GetServicesByCategory(int categoryId)
        {
            return _serviceDAO.GetServicesByCategoryId(categoryId);
        }

        // Add a new service
        public void AddService(Service service)
        {
            _serviceDAO.AddService(service);
        }

        // Update an existing service
        public void UpdateService(Service service)
        {
            _serviceDAO.UpdateService(service);
        }

        // Delete a service
        public void DeleteService(int serviceId)
        {
            _serviceDAO.DeleteService(serviceId);
        }

    }
}