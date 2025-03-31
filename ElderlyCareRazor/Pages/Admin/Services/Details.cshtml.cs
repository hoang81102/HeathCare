using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System;

namespace ElderlyCareRazor.Pages.Admin.Services
{
    public class DetailsModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _categoryService;

        public DetailsModel(IServiceService serviceService, IServiceCategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;
        }

        public Service Service { get; set; }
        public string CategoryName { get; set; }

        public IActionResult OnGet(int id)
        {
            Service = _serviceService.GetServiceById(id);

            if (Service == null)
            {
                return NotFound();
            }

            var category = _categoryService.GetCategoryById(Service.CategoryId);
            CategoryName = category?.CategoryName ?? "Unknown";

            return Page();
        }
    }
}