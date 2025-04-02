using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;

namespace DataAccessObjects
{
    public class ServiceCategoryDAO : SingletonBase<ServiceCategoryDAO>
    {
        private readonly ElderCareContext _dbContext;

        public ServiceCategoryDAO()
        {
            _dbContext = _context;
        }

        // Get all service categories
        public List<ServiceCategory> GetAllCategories()
        {
            try
            {
                return _dbContext.ServiceCategories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving service categories: " + ex.Message);
            }
        }

        // Get service category by ID
        public ServiceCategory GetCategoryById(int categoryId)
        {
            try
            {
                return _dbContext.ServiceCategories
                    .FirstOrDefault(c => c.CategoryId == categoryId)
                    ?? throw new Exception($"Service category with ID {categoryId} not found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving service category: " + ex.Message);
            }
        }

        // Get service category with its services
        public ServiceCategory GetCategoryWithServices(int categoryId)
        {
            try
            {
                return _dbContext.ServiceCategories
                    .Include(c => c.Services)
                    .FirstOrDefault(c => c.CategoryId == categoryId)
                    ?? throw new Exception($"Service category with ID {categoryId} not found");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving service category with services: " + ex.Message);
            }
        }

        // Add a new service category
        public void AddCategory(ServiceCategory category)
        {
            try
            {
                // Validate category name based on CHECK constraint in database
                if (!IsValidCategoryName(category.CategoryName))
                {
                    throw new Exception("Category name must be one of: 'Health Care', 'Personal Care', 'Emergency Care'");
                }

                _dbContext.ServiceCategories.Add(category);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding service category: " + ex.Message);
            }
        }

        // Update an existing service category
        public void UpdateCategory(ServiceCategory category)
        {
            try
            {
                // Validate category name based on CHECK constraint in database
                if (!IsValidCategoryName(category.CategoryName))
                {
                    throw new Exception("Category name must be one of: 'Health Care', 'Personal Care', 'Emergency Care'");
                }

                var existingCategory = _dbContext.ServiceCategories.Find(category.CategoryId)
                    ?? throw new Exception($"Service category with ID {category.CategoryId} not found");

                existingCategory.CategoryName = category.CategoryName;

                _dbContext.Entry(existingCategory).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating service category: " + ex.Message);
            }
        }

        // Delete a service category
        public void DeleteCategory(int categoryId)
        {
            try
            {
                var category = _dbContext.ServiceCategories.Find(categoryId)
                    ?? throw new Exception($"Service category with ID {categoryId} not found");

                // Check if category has services
                if (_dbContext.Services.Any(s => s.CategoryId == categoryId))
                {
                    throw new Exception("Cannot delete category as it has associated services");
                }

                _dbContext.ServiceCategories.Remove(category);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting service category: " + ex.Message);
            }
        }

        // Get categories with service count
        public List<(ServiceCategory Category, int ServiceCount)> GetCategoriesWithServiceCount()
        {
            try
            {
                var result = new List<(ServiceCategory Category, int ServiceCount)>();

                var categories = _dbContext.ServiceCategories.ToList();
                foreach (var category in categories)
                {
                    int count = _dbContext.Services.Count(s => s.CategoryId == category.CategoryId);
                    result.Add((category, count));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving categories with service count: " + ex.Message);
            }
        }

        // Helper method to validate category names based on CHECK constraint
        private bool IsValidCategoryName(string categoryName)
        {
            string[] validNames = { "Health Care", "Personal Care", "Emergency Care" };
            return validNames.Contains(categoryName);
        }
    }
}