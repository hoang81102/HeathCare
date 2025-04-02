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
    public class DetailsModel : PageModel
    {
        private readonly IElderService _elderService;
        private readonly ITrackingService _trackingService;
        private readonly IRecordService _recordService;

        public DetailsModel(
            IElderService elderService,
            ITrackingService trackingService,
            IRecordService recordService)
        {
            _elderService = elderService;
            _trackingService = trackingService;
            _recordService = recordService;
        }

        public Elder Elder { get; set; }
        public List<Tracking> RecentTrackings { get; set; } = new List<Tracking>();
        public List<Record> RecentRecords { get; set; } = new List<Record>();

        public IActionResult OnGet(int id)
        {
            var accountId = HttpContext.Session.GetInt32("AccountId");
            if (accountId == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Get the elder details
            Elder = _elderService.GetElderById(id);

            // Check if elder exists and belongs to the current user
            if (Elder == null || Elder.AccountId != accountId)
            {
                return Page(); // Will show the "Elder not found" message
            }

            // Get recent health tracking data (last 5 entries)
            RecentTrackings = _trackingService.GetTrackingsByElderId(id)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            // Get recent care records (last 5 entries)
            RecentRecords = _recordService.GetRecordsByElderId(id)
                .OrderByDescending(r => r.LastUpdated)
                .Take(5)
                .ToList();

            return Page();
        }

        public string GetStatusBadgeClass(string status)
        {
            return status switch
            {
                "Accepted" => "bg-info",
                "InProgress" => "bg-primary",
                "Finished" => "bg-success",
                _ => "bg-secondary"
            };
        }
    }
}