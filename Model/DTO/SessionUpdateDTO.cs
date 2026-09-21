namespace CourtSideAPI.Model.DTO
{
    public class SessionUpdateDTO
    {
        public string? Title { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? Location { get; set; }
        public int? Minutes { get; set; }
        public string? RecurrenceRule { get; set; }
        public DateTime? RecurrenceEndDate { get; set; }
        public string? Note { get; set; }
    }
}
