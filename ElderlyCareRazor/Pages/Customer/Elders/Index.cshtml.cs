using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;

namespace ElderlyCareRazor.Pages.Customer.Elders
{
    public class IndexModel : PageModel
    {
        private readonly IElderService _elderService;

        public IndexModel(IElderService elderService)
        {
            _elderService = elderService;
        }

        public List<Elder> Elders { get; set; } = new List<Elder>();

        public IActionResult OnGet()
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            Elders = _elderService.GetEldersByAccountId(accountId.Value);
            return Page();
        }
    }
}