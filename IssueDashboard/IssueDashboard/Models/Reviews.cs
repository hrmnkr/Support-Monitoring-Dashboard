namespace IssueDashboard.Models
{
    public class Review
    {
        public int SrNo { get; set; }
        public DateTime ReviewDate { get; set; }
        public string DeviceName { get; set; }
        public string UserName { get; set; }
        public string ReviewText { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string ReplyText { get; set; }
        public string Platform { get; set; } // Android / iOS

        // Derived
        public bool IsReplied => !string.IsNullOrWhiteSpace(ReplyText);
    }
}