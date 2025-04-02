using System;
using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        // Get all services
        public List<Service> GetAllServices()
        {
            return _serviceRepository.GetAllServices();
        }

        // Get service by ID
        public Service GetServiceById(int serviceId)
        {
            return _serviceRepository.GetServiceById(serviceId);
        }

        // Get services by category
        public List<Service> GetServicesByCategory(int categoryId)
        {
            return _serviceRepository.GetServicesByCategory(categoryId);
        }

        // Add a new service
        public void AddService(Service service)
        {
            _serviceRepository.AddService(service);
        }

        // Update an existing service
        public void UpdateService(Service service)
        {
            _serviceRepository.UpdateService(service);
        }

        // Delete a service
        public void DeleteService(int serviceId)
        {
            _serviceRepository.DeleteService(serviceId);
        }

    }
}