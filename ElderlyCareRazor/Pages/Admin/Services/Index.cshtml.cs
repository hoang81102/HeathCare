using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Collections.Generic;

namespace ElderlyCareRazor.Pages.Admin.Services
{
    public class IndexModel : PageModel
    {
        private readonly IServiceService _serviceService;

        public List<Service> ServiceList { get; set; }

        public IndexModel(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        public IActionResult OnGet()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == null || (userRole != "admin" && userRole != "care"))
            {
                TempData["ErrorMessage"] = "You do not have permission to access this page.";
                return RedirectToPage("/Auth/Login");
            }

            ServiceList = _serviceService.GetAllServices();
            return Page();
        }
    }
}
