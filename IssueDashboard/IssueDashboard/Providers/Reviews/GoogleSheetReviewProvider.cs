using IssueDashboard.Models;
using System.Globalization;

namespace IssueDashboard.Providers.Reviews
{
    public class GoogleSheetReviewProvider : IReviewProvider
    {
        private readonly string androidUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSlMf4EfrbcX9rt_siyejEIyd39eSSI18i3vSlc77G2G4AWiwe8tT0KmyPBsokPY3Ooc9vi1oGW8yD0/pub?gid=0&single=true&output=csv";
        private readonly string iosUrl = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSlMf4EfrbcX9rt_siyejEIyd39eSSI18i3vSlc77G2G4AWiwe8tT0KmyPBsokPY3Ooc9vi1oGW8yD0/pub?gid=1670457556&single=true&output=csv";

        public List<Review> GetReviews()
        {
            Console.WriteLine("GoogleSheetReviewProvider is running...");
            var reviews = new List<Review>();

            reviews.AddRange(ReadSheet(androidUrl, "Android"));
            reviews.AddRange(ReadSheet(iosUrl, "iOS"));

            Console.WriteLine($"Total reviews fetched: {reviews.Count}");

            return reviews;
        }

        private List<Review> ReadSheet(string url, string platform)
        {
            var list = new List<Review>();

            using (var client = new HttpClient())
            {
                var csvData = client.GetStringAsync(url).Result;

                Console.WriteLine("===== RAW CSV DATA =====");
                Console.WriteLine(csvData);

                using (var reader = new StringReader(csvData))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ReviewCsvMap>();

                    var records = csv.GetRecords<ReviewCsvRow>().ToList();

                    foreach (var row in records)
                    {
                        DateTime reviewDate;
                        DateTime.TryParse(row.ReviewDate, out reviewDate);

                        DateTime replyDate;
                        DateTime? parsedReplyDate = null;
                        if (DateTime.TryParse(row.ReplyDate, out replyDate))
                            parsedReplyDate = replyDate;

                        list.Add(new Review
                        {
                            SrNo = int.TryParse(row.SrNo, out int sr) ? sr : 0,
                            ReviewDate = reviewDate,
                            DeviceName = row.DeviceName,
                            UserName = row.Name,
                            ReviewText = row.Review,
                            ReplyDate = parsedReplyDate,
                            ReplyText = row.ReplyByHarsac,
                            Platform = platform
                        });
                    }
                }
            }

            return list;
        }
    }
}