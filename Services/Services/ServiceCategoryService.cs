using System;
using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class ServiceCategoryService : IServiceCategoryService
    {
        private readonly IServiceCategoryRepository _serviceCategoryRepository;

        public ServiceCategoryService(IServiceCategoryRepository serviceCategoryRepository)
        {
            _serviceCategoryRepository = serviceCategoryRepository;
        }

        // Get all service categories
        public List<ServiceCategory> GetAllCategories()
        {
            return _serviceCategoryRepository.GetAllCategories();
        }

        // Get service category by ID
        public ServiceCategory GetCategoryById(int categoryId)
        {
            return _serviceCategoryRepository.GetCategoryById(categoryId);
        }

        // Get service category with its services
        public ServiceCategory GetCategoryWithServices(int categoryId)
        {
            return _serviceCategoryRepository.GetCategoryWithServices(categoryId);
        }

        // Add a new service category
        public void AddCategory(ServiceCategory category)
        {
            _serviceCategoryRepository.AddCategory(category);
        }

        // Update an existing service category
        public void UpdateCategory(ServiceCategory category)
        {
            _serviceCategoryRepository.UpdateCategory(category);
        }

        // Delete a service category
        public void DeleteCategory(int categoryId)
        {
            _serviceCategoryRepository.DeleteCategory(categoryId);
        }

        // Get categories with service count
        public List<(ServiceCategory Category, int ServiceCount)> GetCategoriesWithServiceCount()
        {
            return _serviceCategoryRepository.GetCategoriesWithServiceCount();
        }
    }
}