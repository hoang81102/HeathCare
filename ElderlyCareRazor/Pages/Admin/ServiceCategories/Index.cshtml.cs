using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Admin.ServiceCategories
{
    public class IndexModel : PageModel
    {
        private readonly IServiceCategoryService _serviceCategoryService;

        public IndexModel(IServiceCategoryService serviceCategoryService)
        {
            _serviceCategoryService = serviceCategoryService;
        }

        public List<ServiceCategory> Categories { get; set; }
        public List<(ServiceCategory Category, int ServiceCount)> CategoriesWithCount { get; set; }

        public IActionResult OnGet()
        {

            try
            {
                Categories = _serviceCategoryService.GetAllCategories();
                CategoriesWithCount = _serviceCategoryService.GetCategoriesWithServiceCount();
                return Page();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error retrieving service categories: {ex.Message}";
                return Page();
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                _serviceCategoryService.DeleteCategory(id);
                TempData["SuccessMessage"] = "Service category deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting service category: {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}