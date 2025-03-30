using System;
using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class ServiceCategoryRepository : IServiceCategoryRepository
    {
        private readonly ServiceCategoryDAO _serviceCategoryDAO;

        public ServiceCategoryRepository()
        {
            _serviceCategoryDAO = ServiceCategoryDAO.Instance;
        }

        // Get all service categories
        public List<ServiceCategory> GetAllCategories()
        {
            return _serviceCategoryDAO.GetAllCategories();
        }

        // Get service category by ID
        public ServiceCategory GetCategoryById(int categoryId)
        {
            return _serviceCategoryDAO.GetCategoryById(categoryId);
        }

        // Get service category with its services
        public ServiceCategory GetCategoryWithServices(int categoryId)
        {
            return _serviceCategoryDAO.GetCategoryWithServices(categoryId);
        }

        // Add a new service category
        public void AddCategory(ServiceCategory category)
        {
            _serviceCategoryDAO.AddCategory(category);
        }

        // Update an existing service category
        public void UpdateCategory(ServiceCategory category)
        {
            _serviceCategoryDAO.UpdateCategory(category);
        }

        // Delete a service category
        public void DeleteCategory(int categoryId)
        {
            _serviceCategoryDAO.DeleteCategory(categoryId);
        }

        // Get categories with service count
        public List<(ServiceCategory Category, int ServiceCount)> GetCategoriesWithServiceCount()
        {
            return _serviceCategoryDAO.GetCategoriesWithServiceCount();
        }
    }
}