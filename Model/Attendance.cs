namespace CourtSideAPI.Model
{
    public class Attendance
    {
        public Guid AttendanceId { get; set; } = Guid.NewGuid();

        public Guid SessionId { get; set; }
        public Session Session { get; set; }

        public Guid PlayerId { get; set; }
        public Player Player { get; set; }

        public bool IsPresent { get; set; }
    }
}
