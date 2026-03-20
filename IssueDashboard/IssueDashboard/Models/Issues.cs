namespace IssueDashboard.Models
{
    public class Issue
    {
        public int SrNo { get; set; }
        public string Source { get; set; }
        public DateTime Date { get; set; }
        //public string Date { get; set; }
        public string ReportedBy { get; set; }
        public string IssueText { get; set; }
        public string ResolveBy { get; set; }
        public string Resolution { get; set; }
        public string Status { get; set; }
        //public string Designation { get; set; }
        //public string ReporterName { get; set; }
        public string CleanReporter { get; set; }
    }
}