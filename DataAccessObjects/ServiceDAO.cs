using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace DataAccessObjects
{
    public class ServiceDAO : SingletonBase<ServiceDAO>
    {
        private readonly ElderCareContext _dbContext;

        public ServiceDAO()
        {
            _dbContext = _context;
        }

        // Get all services
        public List<Service> GetAllServices()
        {
            try
            {
                return _dbContext.Services
                    .Include(s => s.Category)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving services: " + ex.Message);
            }
        }

        // Get service by ID
        public Service GetServiceById(int serviceId)
        {
            try
            {
                return _dbContext.Services
                    .Include(s => s.Category)
                    .FirstOrDefault(s => s.ServiceId == serviceId)
                    ?? throw new Exception($"Service with ID {serviceId} not found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving service: " + ex.Message);
            }
        }

        // Get services by category ID
        public List<Service> GetServicesByCategoryId(int categoryId)
        {
            try
            {
                return _dbContext.Services
                    .Include(s => s.Category)
                    .Where(s => s.CategoryId == categoryId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving services by category: " + ex.Message);
            }
        }

        // Add a new service
        public void AddService(Service service)
        {
            try
            {
                _dbContext.Services.Add(service);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding service: " + ex.Message);
            }
        }

        // Update an existing service
        public void UpdateService(Service service)
        {
            try
            {
                var existingService = _dbContext.Services.Find(service.ServiceId)
                    ?? throw new Exception($"Service with ID {service.ServiceId} not found");

                // Update properties
                existingService.ServiceName = service.ServiceName;
                existingService.Description = service.Description;
                existingService.Price = service.Price;
                existingService.CategoryId = service.CategoryId;

                _dbContext.Entry(existingService).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating service: " + ex.Message);
            }
        }

        // Delete a service
        public void DeleteService(int serviceId)
        {
            try
            {
                var service = _dbContext.Services.Find(serviceId)
                    ?? throw new Exception($"Service with ID {serviceId} not found");

                // Check if service is used in any bookings
                if (_dbContext.Bookings.Any(b => b.ServiceId == serviceId))
                {
                    throw new Exception("Cannot delete service as it is referenced in existing bookings");
                }

                _dbContext.Services.Remove(service);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting service: " + ex.Message);
            }
        }

        // Search services by name or description
        public List<Service> SearchServices(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return GetAllServices();

                return _dbContext.Services
                    .Include(s => s.Category)
                    .Where(s => s.ServiceName.Contains(searchTerm) ||
                                (s.Description != null && s.Description.Contains(searchTerm)))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching services: " + ex.Message);
            }
        }

        // Get services with price range
        public List<Service> GetServicesInPriceRange(decimal minPrice, decimal maxPrice)
        {
            try
            {
                return _dbContext.Services
                    .Include(s => s.Category)
                    .Where(s => s.Price >= minPrice && s.Price <= maxPrice)
                    .OrderBy(s => s.Price)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving services by price range: " + ex.Message);
            }
        }
    }
}