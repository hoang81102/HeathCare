using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;

namespace ElderlyCareRazor.Pages.Admin.Services
{
    public class IndexModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _categoryService;

        public IndexModel(IServiceService serviceService, IServiceCategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;
        }

        public List<Service> Services { get; set; } = new List<Service>();
        private Dictionary<int, string> CategoryNames { get; set; } = new Dictionary<int, string>();

        public void OnGet()
        {
            Services = _serviceService.GetAllServices();
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = _categoryService.GetAllCategories();
            CategoryNames.Clear();
            foreach (var category in categories)
            {
                CategoryNames[category.CategoryId] = category.CategoryName;
            }
        }

        public string GetCategoryName(int categoryId)
        {
            if (CategoryNames.ContainsKey(categoryId))
            {
                return CategoryNames[categoryId];
            }
            return "Unknown";
        }
    }
}