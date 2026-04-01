using CsvHelper.Configuration;
using IssueDashboard.Models;

namespace IssueDashboard.Providers.Reviews
{
    public sealed class ReviewCsvMap : ClassMap<ReviewCsvRow>
    {
        public ReviewCsvMap()
        {
            Map(m => m.SrNo).Name("Sr No");
            Map(m => m.ReviewDate).Name("Review Date");
            Map(m => m.DeviceName).Name("Device Name");
            Map(m => m.Name).Name("Name");
            Map(m => m.Review).Name("Review");
            Map(m => m.ReplyDate).Name("Reply Date");
            Map(m => m.ReplyByHarsac).Name("Reply by Harsac");
        }
    }
}
