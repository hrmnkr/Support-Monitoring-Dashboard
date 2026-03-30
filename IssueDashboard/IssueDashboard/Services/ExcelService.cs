using IssueDashboard.Models;
using OfficeOpenXml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IssueDashboard.Services
{
    public class ExcelService
    {
        public List<Issue> GetIssues(string filePath)
        {
            ExcelPackage.License.SetNonCommercialPersonal("IssueDashboard");

            var issues = new List<Issue>();

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var sheet = package.Workbook.Worksheets[0];
                int rows = sheet.Dimension.Rows;

                for (int row = 2; row <= rows; row++)
                {
                    // Skip completely empty rows
                    if (string.IsNullOrWhiteSpace(sheet.Cells[row, 1].Text))
                        continue;

                    string rawSource = sheet.Cells[row, 1].Text?.Trim().ToLower();

                    // Normalize source
                    string normalizedSource = "";

                    if (!string.IsNullOrEmpty(rawSource))
                    {
                        if (rawSource.Contains("mail") || rawSource.Contains("email"))
                            normalizedSource = "Mail";
                        else if (rawSource.Contains("web"))
                            normalizedSource = "WebSupport";
                        else if (rawSource.Contains("whatsapp") || rawSource.Contains("whtasapp"))
                            normalizedSource = "WhatsApp";
                        else
                            normalizedSource = rawSource; // fallback (optional)
                    }

                    // Safe SrNo parsing
                    int srNo = 0;
                    int.TryParse(sheet.Cells[row, 2].Text, out srNo);

                    //Safe Date parsing
                    DateTime dateValue;

                    // Try multiple formats + fallback
                    var rawDate = sheet.Cells[row, 3].Text?.Trim();

                    bool isValidDate = DateTime.TryParseExact(
                        rawDate,
                        new[] { "dd-MM-yyyy", "dd/MM/yyyy", "MM/dd/yyyy" },
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out dateValue
                    );

                    // Fallback: general parse (for Excel date formats)
                    if (!isValidDate)
                    {
                        DateTime.TryParse(rawDate, out dateValue);
                    }
                    if (dateValue == DateTime.MinValue)
                    {
                        Console.WriteLine($"Invalid date at row {row}: {rawDate}");
                        continue;
                    }

                    string rawReporter = sheet.Cells[row, 4].Text;
                    string cleanReporter = rawReporter;

                    // MAIL → extract before @
                    if (sheet.Cells[row, 1].Text == "Mail")
                    {
                        var match = Regex.Match(rawReporter, @"^[^@]+");
                        if (match.Success)
                            cleanReporter = match.Value;
                    }

                    // WebSupport & WhatsApp → keep full name


                    issues.Add(new Issue
                    {
                        SrNo = srNo,
                        Source = normalizedSource,
                        Date = dateValue,
                        CleanReporter = cleanReporter,
                        ReportedBy = rawReporter,
                        IssueText = sheet.Cells[row, 6].Text,
                        ResolveBy = sheet.Cells[row, 7].Text,
                        Resolution = sheet.Cells[row, 8].Text,
                        Status = sheet.Cells[row, 9].Text
                    });
                }
            }

            return issues;
        }
    }
}