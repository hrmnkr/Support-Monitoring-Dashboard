using IssueDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssueDashboard.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string sourceFilter, string reporterFilter, int page = 1, int pageSize = 10)
        {
            var service = new ExcelService();

            string path = Path.Combine(Directory.GetCurrentDirectory(),
            "Data/Mail_Issues_ and_Reply.xlsx");

            var issues = service.GetIssues(path);

            //Filters
            if (!string.IsNullOrEmpty(sourceFilter))
                issues = issues.Where(x => x.Source == sourceFilter).ToList();

            if (!string.IsNullOrEmpty(reporterFilter))
                issues = issues.Where(x => x.ReportedBy == reporterFilter).ToList();

            // KPI Counts
            ViewBag.TotalIssues = issues.Count;

            ViewBag.MailIssues = issues.Count(x => x.Source == "Mail");
            ViewBag.WebSupportIssues = issues.Count(x => x.Source == "WebSupport");
            ViewBag.WhatsAppIssues = issues.Count(x => x.Source == "WhatsApp");

            ViewBag.ResolvedCount = issues.Count(x => x.Status == "Resolved");
            ViewBag.PendingCount = issues.Count(x => x.Status != "Resolved");

            // Source-wise KPI
            ViewBag.MailResolved = issues.Count(x => x.Source == "Mail" && x.Status == "Resolved");
            ViewBag.MailPending = issues.Count(x => x.Source == "Mail" && x.Status != "Resolved");

            ViewBag.WebResolved = issues.Count(x => x.Source == "WebSupport" && x.Status == "Resolved");
            ViewBag.WebPending = issues.Count(x => x.Source == "WebSupport" && x.Status != "Resolved");

            ViewBag.WaResolved = issues.Count(x => x.Source == "WhatsApp" && x.Status == "Resolved");
            ViewBag.WaPending = issues.Count(x => x.Source == "WhatsApp" && x.Status != "Resolved");

            // NOTE:
            // "Today's Issues" KPI is currently disabled because data is sourced from static Excel.
            // Since the Excel file is not auto-updated in real-time, showing today's data may be inaccurate.
            // This will be enabled once data source becomes dynamic or auto-refreshed.

            //ViewBag.TodayIssues = issues.Count(x => x.Date.Date == DateTime.Today);

            ViewBag.SourceData = issues
                .GroupBy(x => x.Source)
                .Select(g => new { Source = g.Key, Count = g.Count() })
                .ToList();

            ViewBag.ReporterData = issues
                .GroupBy(x => x.CleanReporter)
                .Select(g => new { Reporter = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToList();

            ViewBag.TrendData = issues
                .GroupBy(x => x.Date.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd-MMM"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            ViewBag.SourceList = issues.Select(x => x.Source).Distinct().ToList();
            ViewBag.ReporterList = issues.Select(x => x.ReportedBy).Distinct().ToList();

            var totalRecords = issues.Count;

            var paginatedData = issues
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            // For showing range
            ViewBag.StartRecord = (page - 1) * pageSize + 1;
            ViewBag.EndRecord = Math.Min(page * pageSize, totalRecords);

            // Buttons
            ViewBag.HasPrevious = page > 1;
            ViewBag.HasNext = page < ViewBag.TotalPages;

            return View(paginatedData);
        }
    }
}