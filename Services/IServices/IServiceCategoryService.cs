using System;
using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface IServiceCategoryService
    {
        // Get all service categories
        List<ServiceCategory> GetAllCategories();

        // Get service category by ID
        ServiceCategory GetCategoryById(int categoryId);

        // Get service category with its services
        ServiceCategory GetCategoryWithServices(int categoryId);

        // Add a new service category
        void AddCategory(ServiceCategory category);

        // Update an existing service category
        void UpdateCategory(ServiceCategory category);

        // Delete a service category
        void DeleteCategory(int categoryId);

        // Get categories with service count
        List<(ServiceCategory Category, int ServiceCount)> GetCategoriesWithServiceCount();
    }
}