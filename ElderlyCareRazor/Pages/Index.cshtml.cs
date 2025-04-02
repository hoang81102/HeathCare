using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;
using BusinessObjects;

namespace ElderlyCareRazor.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IServiceCategoryService _serviceCategoryService;
        private readonly IServiceService _serviceService;

        public IndexModel(IServiceCategoryService serviceCategoryService, IServiceService serviceService)
        {
            _serviceCategoryService = serviceCategoryService;
            _serviceService = serviceService;
        }

        public List<ServiceCategory> ServiceCategories { get; set; }
        public Dictionary<int, List<Service>> CategoryServices { get; set; }

        public void OnGet()
        {
            // Load service categories and featured services for display
            ServiceCategories = _serviceCategoryService.GetAllCategories();
            CategoryServices = new Dictionary<int, List<Service>>();

            foreach (var category in ServiceCategories)
            {
                // Get services for each category (limiting to 3 per category for featured display)
                var services = _serviceService.GetServicesByCategory(category.CategoryId);
                CategoryServices[category.CategoryId] = services.Count > 3 ? services.GetRange(0, 3) : services;
            }
        }
    }
}