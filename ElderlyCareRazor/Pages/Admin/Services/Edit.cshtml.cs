using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Services;
using System.Collections.Generic;

namespace ElderlyCareRazor.Pages.Admin.Services
{
    public class EditModel : PageModel
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceCategoryService _categoryService;

        public EditModel(IServiceService serviceService, IServiceCategoryService categoryService)
        {
            _serviceService = serviceService;
            _categoryService = categoryService;
        }

        [BindProperty]
        public Service Service { get; set; }

        public SelectList Categories { get; set; }

        public IActionResult OnGet(int id)
        {
            Service = _serviceService.GetServiceById(id);

            if (Service == null)
            {
                return NotFound();
            }

            LoadCategories();
            return Page();
        }

        public IActionResult OnPost()
        {
            ModelState.Remove("Service.Category");

            if (!ModelState.IsValid)
            {
                LoadCategories();
                return Page();
            }

            _serviceService.UpdateService(Service);
            TempData["SuccessMessage"] = "Service updated successfully!";

            return RedirectToPage("./Index");
        }

        private void LoadCategories()
        {
            var categories = _categoryService.GetAllCategories();
            Categories = new SelectList(categories, "CategoryId", "CategoryName");
        }
    }
}