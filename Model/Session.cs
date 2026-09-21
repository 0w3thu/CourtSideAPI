namespace CourtSideAPI.Model
{
    public enum Status
    {
        Scheduled, Completed, Cancelled, Postponed
    }

    public class Session
    {
        public Guid SessionId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public string location { get; set; }
        public int Minutes{ get; set; }
        public Status Status{ get; set; }
        public string? recurrenceRule { get; set; }
        public DateTime recurrenceEndDate { get; set; }
        public string? Note { get; set; }

        public Guid TeamId { get; set; }
        public Team Team { get; set; }
    }
}
