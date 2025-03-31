using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System.Collections.Generic;

namespace ElderlyCareRazor.Pages.Admin.Services
{
    public class CreateModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _categoryService;

        public CreateModel(IServiceService serviceService, IServiceCategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Service Service { get; set; } = new Service();

        public SelectList Categories { get; set; }

        public void OnGet()
        {
            LoadCategories();
        }

        public IActionResult OnPost()
        {
            ModelState.Remove("Service.Category");
            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            _serviceService.AddService(Service);
            TempData["SuccessMessage"] = "Service created successfully!";

            return RedirectToPage("./Index");
        }

        private void LoadCategories()
        {
            var categories = _categoryService.GetAllCategories();
            Categories = new SelectList(categories, "CategoryId", "CategoryName");
        }
    }
}