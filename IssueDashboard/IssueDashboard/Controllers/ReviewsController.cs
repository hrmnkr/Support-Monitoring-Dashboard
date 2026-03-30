using IssueDashboard.Providers.Reviews;
using Microsoft.AspNetCore.Mvc;

namespace IssueDashboard.Controllers
{
    public class ReviewsController : Controller
    {
        public IActionResult Index(string platformFilter)
        {
            IReviewProvider provider = new GoogleSheetReviewProvider();

            var allReviews = provider.GetReviews();
            var filteredReviews = allReviews;

            // Apply filter
            if (!string.IsNullOrEmpty(platformFilter))
            {
                filteredReviews = filteredReviews
                    .Where(x => x.Platform.Equals(platformFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // KPI
            ViewBag.TotalReviews = allReviews.Count;
            ViewBag.AndroidReviews = allReviews.Count(x => x.Platform == "Android");
            ViewBag.IOSReviews = allReviews.Count(x => x.Platform == "iOS");

            ViewBag.Replied = filteredReviews.Count(x => !string.IsNullOrEmpty(x.ReplyText));
            ViewBag.Pending = filteredReviews.Count(x => string.IsNullOrEmpty(x.ReplyText));

            // Chart - Platform
            ViewBag.PlatformData = filteredReviews
                .GroupBy(x => x.Platform)
                .Select(g => new { Platform = g.Key, Count = g.Count() })
                .ToList();

            // Chart - Trend
            ViewBag.TrendData = filteredReviews
                .GroupBy(x => x.ReviewDate.Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("dd-MMM"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            return View(filteredReviews);
        }
    }
}