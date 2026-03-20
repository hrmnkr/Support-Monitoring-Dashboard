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

                    // Safe SrNo parsing
                    int srNo = 0;
                    int.TryParse(sheet.Cells[row, 2].Text, out srNo);

                    //Safe Date parsing
                    DateTime dateValue;
                    DateTime.TryParseExact(
                        sheet.Cells[row, 3].Text,
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out dateValue
                    );
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
                        Source = sheet.Cells[row, 1].Text,
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